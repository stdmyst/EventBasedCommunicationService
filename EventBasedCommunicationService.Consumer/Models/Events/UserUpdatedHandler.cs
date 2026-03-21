using EventBasedCommunicationService.Abstraction;

namespace EventBasedCommunicationService.Consumer.Models.Events;

class UserUpdatedHandler : IEventHandler<UserUpdated>
{
    public Task Handle(UserUpdated @event)
    {
        Console.WriteLine($@"""{typeof(UserUpdatedHandler)}"" handle ""{nameof(UserUpdated)}"" event ""{@event.Id}""");
        
        Console.WriteLine($"{@event.User} has been updated");
        
        return Task.CompletedTask;
    }
}