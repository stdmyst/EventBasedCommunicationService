using System.Reflection;
using EventBasedCommunicationService;
using EventBasedCommunicationService.Implementation;
using EventBasedCommunicationService.Models;
using EventBasedCommunicationService.Publisher.Models.Events;

var settings = ConfigurationHelpers.GetSettings<AppSettings>(Assembly.GetExecutingAssembly());
var eventBus = new EventService(settings.RabbitMqHostname, Assembly.GetExecutingAssembly());

var @event = new UserUpdated
{
    Id = Guid.NewGuid(),
    User = new() { Id = Guid.NewGuid(), Username = "John Doe" }
};

await eventBus.Publish(@event, exchange: "event-based-communication");