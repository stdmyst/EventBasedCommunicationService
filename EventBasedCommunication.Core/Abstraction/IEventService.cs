namespace EventBasedCommunication.Core.Abstraction;

public interface IEventService
{
    public Task Publish<T>(T @event, string exchange, string routingKey) 
        where T : IEvent;
    
    public Task Receive(string exchange, string[] routingKeys);
}