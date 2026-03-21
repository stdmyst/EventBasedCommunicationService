namespace EventBasedCommunicationService.Abstraction;

public interface IEventService
{
    public Task Publish<T>(T @event, string exchange) 
        where T : IEvent;
    
    public Task Receive(string exchange, CancellationToken cancellationToken);
}