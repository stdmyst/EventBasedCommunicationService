using System.Reflection;
using System.Text;
using System.Text.Json;
using EventBasedCommunicationService.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBasedCommunicationService.Implementation;

public class EventService(string host, Assembly assembly, IServiceProvider services, TimeSpan? timeout = null)
    : IPublisher, ISubscriber
{
    private readonly TimeSpan _timeout = timeout ?? TimeSpan.FromSeconds(1);
    private readonly ConnectionFactory _connectionFactory = new() { HostName = host };
    private readonly EventResolver _eventResolver = new(assembly, services);
    private readonly ILogger<EventService> _logger = services.GetService<ILogger<EventService>>()
                                                     ?? throw new NullReferenceException("No logger defined.");

    public async Task Publish<T>(T @event, string exchange) 
        where T : IEvent
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct);
        
        var message = JsonSerializer.Serialize(@event);
        var eventBytes = Encoding.UTF8.GetBytes(message);
        
        await channel.BasicPublishAsync(exchange, routingKey: @event.RoutingKey, body: eventBytes);
        
        _logger.LogInformation(@"Sent message with ""{RoutingKey}"" routing key to ""{Exchange}"" exchange: {Message}",
            @event.RoutingKey, exchange, message);
    }

    public async Task Subscribe(string exchange, CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct, cancellationToken: cancellationToken);
        
        var queue  = await channel.QueueDeclareAsync(durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        foreach (var routingKey in _eventResolver.RoutingKeys)
        {
            _logger.LogInformation(@"Binding ""{Exchange}"" exchange with queue: ""{Queue}"" by binding key: ""{BindingKey}""",
                exchange, queue.QueueName, routingKey);
            
            await channel.QueueBindAsync(queue.QueueName, exchange, routingKey, cancellationToken: cancellationToken);
        }

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, eventArgs) => await Handle(eventArgs.RoutingKey, eventArgs.Body.ToArray());

        _logger.LogInformation("Waiting for messages...");
        
        await channel.BasicConsumeAsync(queue.QueueName, autoAck: true, consumer, cancellationToken: cancellationToken);
        
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            cancellationToken.WaitHandle.WaitOne(_timeout);
        }
    }

    private async Task Handle(string routingKey, byte[] message)
    {
        var consumers = _eventResolver.GetEvents(routingKey);
        if (consumers.Length == 0) return;
        
        var body = Encoding.UTF8.GetString(message);
        
        foreach (var consumer in consumers)
        {
            var @event = JsonSerializer.Deserialize(body, consumer) as IEvent;
            if (@event is null) continue;
            
            var handlers = _eventResolver.GetHandlers(consumer);
            if (handlers.Length == 0)
            {
                _logger.LogInformation(@"No registered handlers found for ""{Type}"".", @event.GetType().FullName);
                continue;
            }
            
            foreach (var handler in handlers)
                await handler(@event);
        }
    }
}