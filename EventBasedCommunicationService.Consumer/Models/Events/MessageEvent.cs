using EventBasedCommunicationService.Models;

namespace EventBasedCommunicationService.Consumer.Models.Events;

[EventSubscribe]
public class MessageEvent() : Event(Key)
{
    private const string Key = "message.event";

    public bool IsLast { get; set; }
    
    public Guid UserId { get; init; }

    public string Message { get; init; }
}