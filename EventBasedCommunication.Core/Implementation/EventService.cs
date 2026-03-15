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

    public async Task Publish<T>(T @event, string exchange, string routingKey) 
        where T : IEvent
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct);
        
        var eventMessage = JsonSerializer.Serialize(@event);
        var eventBytes = Encoding.UTF8.GetBytes(eventMessage);
        await channel.BasicPublishAsync(exchange, routingKey: routingKey, body:eventBytes);
        
        Console.WriteLine($"Sent to {exchange} exchange {eventMessage}.");
    }

    public async Task Receive(string exchange, string[] routingKeys)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct);
        
        var queue  = await channel.QueueDeclareAsync();
        foreach (var routingKey in  routingKeys)
        {
            await channel.QueueBindAsync(queue.QueueName, exchange, routingKey);
        }
        
        Console.WriteLine("Waiting for messages.");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, eventArgs) =>
        {
            var message = JsonSerializer.Deserialize<Event>(eventArgs.Body.ToArray());
            Console.WriteLine($"Received from {queue.QueueName}: {JsonSerializer.Serialize(message)}");
            
            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(queue.QueueName, autoAck: true, consumer);
        
        Console.WriteLine(" Press any key to exit.");
        Console.ReadLine();
    }
}