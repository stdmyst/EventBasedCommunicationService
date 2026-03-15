using EventBasedCommunication.Core.Models;

namespace EventBasedCommunication.Core.Abstraction;

public interface IEventService
{
    public Task Publish<T>(T @event, string exchange) where T : IEvent;
    
    public Task Receive(string exchange);
}