using System.Reflection;
using System.Text;
using System.Text.Json;
using EventBasedCommunicationService.Abstraction;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBasedCommunicationService.Implementation;

public class EventService(string host, Assembly assembly, TimeSpan? timeout = null) : IEventService
{
    private readonly TimeSpan _timeout = timeout ?? TimeSpan.FromSeconds(1);
    private readonly ConnectionFactory _connectionFactory = new() { HostName = host };
    private readonly EventScanner _eventScanner = new(assembly);
    
    public async Task Publish<T>(T @event, string exchange) 
        where T : IEvent
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct);
        
        var eventMessage = JsonSerializer.Serialize(@event);
        var eventBytes = Encoding.UTF8.GetBytes(eventMessage);
        
        await channel.BasicPublishAsync(exchange, routingKey: @event.RoutingKey, body:eventBytes);
        
        Console.WriteLine($@"Sent message with ""{@event.RoutingKey}"" routing key to ""{exchange}"" exchange: {eventMessage}");
    }

    public async Task Receive(string exchange, CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct, cancellationToken: cancellationToken);
        
        var queue  = await channel.QueueDeclareAsync(durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        foreach (var routingKey in _eventScanner.RoutingKeys)
        {
            Console.WriteLine($@"Binding ""{exchange}"" exchange with queue: ""{queue.QueueName}"" by binding key: ""{routingKey}""");
            await channel.QueueBindAsync(queue.QueueName, exchange, routingKey, cancellationToken: cancellationToken);
        }

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            await Handle(eventArgs.RoutingKey, eventArgs.Body.ToArray());
        };

        Console.WriteLine("Waiting for messages...");
        
        await channel.BasicConsumeAsync(queue.QueueName, autoAck: true, consumer, cancellationToken: cancellationToken);
        
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            cancellationToken.WaitHandle.WaitOne(_timeout);
        }
    }

    private async Task Handle(string routingKey, byte[] message)
    {
        var consumers = _eventScanner.GetEvents(routingKey);
        if (consumers.Length == 0) return;
        
        var body = Encoding.UTF8.GetString(message);
        
        foreach (var consumer in consumers)
        {
            var @event = JsonSerializer.Deserialize(body, consumer) as IEvent;
            if (@event is null) continue;
            
            var handlers = _eventScanner.GetHandlers(consumer);
            if (handlers.Length == 0) continue;
            foreach (var handler in handlers)
                await handler(@event);
        }
    }
}