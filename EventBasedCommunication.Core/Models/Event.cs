using EventBasedCommunication.Core.Abstraction;

namespace EventBasedCommunication.Core.Models;

public class Event : IEvent
{
    public Guid Id { get; init; }
    
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}