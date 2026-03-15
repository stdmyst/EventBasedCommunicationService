using EventBasedCommunication.Core.Models;

namespace EventBasedCommunication.Core.Abstraction;

public interface IEventService
{
    public Task Publish<T>(T @event, string queue, string routingKey) where T : IEvent;
    
    public Task Receive(string queue, string routingKey);
}