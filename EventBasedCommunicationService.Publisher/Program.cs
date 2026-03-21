using System.Reflection;
using EventBasedCommunicationService;
using EventBasedCommunicationService.Implementation;
using EventBasedCommunicationService.Models;
using EventBasedCommunicationService.Publisher.Models.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var assembly = Assembly.GetExecutingAssembly();
var settings = ConfigurationHelpers.GetSettings<AppSettings>(Assembly.GetExecutingAssembly());

var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging(builder => builder.AddConsole());
var services = serviceCollection.BuildServiceProvider();

var eventBus = new EventService(settings.RabbitMqHostname, assembly, services);

var @event = new UserUpdated
{
    Id = Guid.NewGuid(),
    User = new() { Id = Guid.NewGuid(), Username = "John Doe" }
};

await eventBus.Publish(@event, exchange: "event-based-communication");