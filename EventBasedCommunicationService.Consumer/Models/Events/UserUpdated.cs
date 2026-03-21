using EventBasedCommunicationService.Models;

namespace EventBasedCommunicationService.Consumer.Models.Events;

[EventSubscribe]
class UserUpdated() : Event(Key)
{
    private const string Key = "user.updated";
    
    public required User User { get; init; }
}