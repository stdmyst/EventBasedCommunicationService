using System.Reflection;
using EventBasedCommunicationService;
using EventBasedCommunicationService.Abstraction;
using EventBasedCommunicationService.Models;
using EventBasedCommunicationService.Publisher.Models.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var assembly = Assembly.GetExecutingAssembly();
var settings = ConfigurationHelpers.GetSettings<EventServiceSettings>(Assembly.GetExecutingAssembly());

var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging(builder => builder.AddConsole());
serviceCollection.AddEventBasedCommunicationService([assembly]);
serviceCollection.AddSingleton(Options.Create(settings));

var services = serviceCollection.BuildServiceProvider();

var eventBus = services.GetService<IPublisher>() 
               ?? throw new NullReferenceException($"{typeof(IPublisher)} service not found.");

var @event = new UserUpdated
{
    Id = Guid.NewGuid(),
    User = new() { Id = Guid.NewGuid(), Username = "John Doe" }
};

await eventBus.Publish(@event, exchange: "event-based-communication");