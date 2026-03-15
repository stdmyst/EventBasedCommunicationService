using System.Reflection;
using EventBasedCommunication.Core;
using EventBasedCommunication.Core.Implementation;
using EventBasedCommunication.Core.Models;

var settings = ConfigurationHelpers.GetSettings<AppSettings>(Assembly.GetExecutingAssembly());
var eventBus = new EventService(settings.RabbitMqHostname);
var @event = new Event { Id = Guid.NewGuid() };

await eventBus.Publish(@event, "hello");