using EventBasedCommunicationService.Abstraction;
using Microsoft.Extensions.Logging;

namespace EventBasedCommunicationService.Consumer.Models.Events;

//class UserUpdatedHandler(ILogger<UserUpdatedHandler> logger) : IEventHandler<UserUpdated>
//{
//    public Task Handle(UserUpdated @event)
//    {
//        logger.LogInformation(@"""{Handler}"" handle ""{EventName}"" event ""{EventId}""",
//            nameof(UserUpdatedHandler), nameof(UserUpdated), @event.Id);
//        
//        logger.LogInformation("{User} has been updated", @event.User);
//        
//        return Task.CompletedTask;
//    }
//}