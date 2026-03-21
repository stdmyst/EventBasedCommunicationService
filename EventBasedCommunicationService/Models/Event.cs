using EventBasedCommunicationService.Abstraction;

namespace EventBasedCommunicationService.Models;

public class Event(string routingKey) : IEvent
{
    public string RoutingKey { get; } = routingKey;

    public Guid Id { get; init; }
    
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}