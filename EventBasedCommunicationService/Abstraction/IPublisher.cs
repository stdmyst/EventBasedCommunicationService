namespace EventBasedCommunicationService.Abstraction;

public interface IPublisher
{
    Task Publish<T>(T @event, string exchange) where T : IEvent;
}