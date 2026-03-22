using System.Text;
using System.Text.Json;
using EventBasedCommunicationService.Abstraction;
using EventBasedCommunicationService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBasedCommunicationService.Implementation;

internal class EventService : IPublisher, ISubscriber
{
    private readonly TimeSpan _timeout;
    private readonly ConnectionFactory _connectionFactory;
    private readonly ILogger<EventService> _logger;
    private readonly IServiceProvider _services;
    private readonly EventManager _eventManager;

    public EventService(EventManager eventManager, 
        IOptions<EventServiceSettings> options, 
        IServiceProvider services, 
        ILogger<EventService> logger)
    {
        var settings = options.Value;
        _timeout = settings.TimeoutSeconds !=  null 
            ? TimeSpan.FromSeconds(settings.TimeoutSeconds.Value) 
            : TimeSpan.FromSeconds(1);
        
        _connectionFactory =  new ConnectionFactory { HostName = settings.RabbitMqHostname };
        _logger = logger;
        _services = services;
        _eventManager = eventManager;
    }

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
        foreach (var routingKey in _eventManager.RoutingKeys)
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
        var subscribers = _eventManager.GetSubscribers(routingKey);
        if (subscribers.Length == 0) return;
        
        var body = Encoding.UTF8.GetString(message);
        
        foreach (var subscriber in subscribers)
        {
            var @event = JsonSerializer.Deserialize(body, subscriber) as IEvent;
            if (@event is null) continue;
            
            var handlerInterfaces = _eventManager.GetHandlers(subscriber);
            if (handlerInterfaces.Length == 0)
            {
                _logger.LogInformation(@"No registered handlers found for ""{Type}"".", @event.GetType().FullName);
                continue;
            }

            foreach (var handlerInterface in handlerInterfaces)
            {
                var handler = _services.GetService(handlerInterface);
                var handlerMethodInfo = handler?.GetType().GetMethod(nameof(IEventHandler<>.Handle));
                if (handler == null || handlerMethodInfo == null) 
                    continue;
                
                await (Task)(handlerMethodInfo.Invoke(handler, parameters: [@event]) 
                             ?? throw new InvalidOperationException());
            }
        }
    }
}