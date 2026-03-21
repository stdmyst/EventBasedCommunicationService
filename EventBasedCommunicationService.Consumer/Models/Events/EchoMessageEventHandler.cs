using EventBasedCommunicationService.Abstraction;
using Microsoft.Extensions.Logging;

namespace EventBasedCommunicationService.Consumer.Models.Events;

public class EchoMessageEventHandler(ILogger<MessageEvent> logger, IPublisher publisher) 
    : IEventHandler<MessageEvent>
{
    private const string Exchange = "event-based-communication";
    
    public async Task Handle(MessageEvent @event)
    {
        logger.LogInformation(@"Received message ""{MessageId}"" (IsLast = {IsLast}) from user ""{UserId}"": ""{Message}""", 
            @event.UserId,  @event.IsLast, @event.Id, @event.Message);

        if (!@event.IsLast)
        {
            @event.IsLast = true;
            
            logger.LogInformation(@"Echo message ""{EventId}"" to ""{Exchange}"" exchange.", @event.Id, Exchange);
            
            await publisher.Publish(@event, Exchange);
        }
        else
        {
            logger.LogInformation(@"It was last step for message ""{EventId}""", @event.Id);
        }
    }
}