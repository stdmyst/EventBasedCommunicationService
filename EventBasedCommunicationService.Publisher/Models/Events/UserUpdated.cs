using EventBasedCommunicationService.Models;

namespace EventBasedCommunicationService.Publisher.Models.Events;

class UserUpdated() : Event(Key)
{
    private const string Key = "user.updated";
    
    public required User User { get; init; }
}