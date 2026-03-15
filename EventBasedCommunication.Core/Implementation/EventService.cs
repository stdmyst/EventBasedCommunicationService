using System.Text;
using System.Text.Json;
using EventBasedCommunication.Core.Abstraction;
using EventBasedCommunication.Core.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBasedCommunication.Core.Implementation;

public class EventService(string host) : IEventService
{
    private readonly ConnectionFactory _connectionFactory = new() { HostName = host };

    public async Task Publish<T>(T @event, string queue, string routingKey) 
        where T : IEvent
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue, false, false, false);
        
        var eventMessage = JsonSerializer.Serialize(@event);
        var eventBytes = Encoding.UTF8.GetBytes(eventMessage);
        await channel.BasicPublishAsync(string.Empty, routingKey, eventBytes);
        
        Console.WriteLine($"Sent to {queue}/{routingKey} {eventMessage}.");
    }

    public async Task Receive(string queue, string routingKey)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue, false, false, false);
        
        Console.WriteLine("Waiting for messages.");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, eventArgs) =>
        {
            var message = JsonSerializer.Deserialize<Event>(eventArgs.Body.ToArray());
            Console.WriteLine($"Received from {queue}/{routingKey}: {JsonSerializer.Serialize(message)}");
            
            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(queue, autoAck: true, consumer);
    }
}