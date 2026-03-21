namespace EventBasedCommunicationService.Abstraction;

public interface IEvent
{
    string RoutingKey { get; }
}