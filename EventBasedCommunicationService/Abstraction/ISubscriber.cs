namespace EventBasedCommunicationService.Abstraction;

public interface ISubscriber
{
    Task Subscribe(string exchange, CancellationToken cancellationToken);
}