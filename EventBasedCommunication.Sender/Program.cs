using System.Reflection;
using EventBasedCommunication.Core;
using EventBasedCommunication.Core.Implementation;
using EventBasedCommunication.Core.Models;

var settings = ConfigurationHelpers.GetSettings<AppSettings>(Assembly.GetExecutingAssembly());
var eventBus = new EventService(settings.RabbitMqHostname);
var @event = new Event { Id = Guid.NewGuid() };

var routingKey = args.Length > 0 ? args[0] : "hello";

await eventBus.Publish(@event, exchange: "hello", routingKey);