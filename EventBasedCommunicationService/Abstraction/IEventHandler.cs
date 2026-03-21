namespace EventBasedCommunicationService.Abstraction;

public interface IEventHandler<in T> where T : IEvent
{
    Task Handle(T @event);
}