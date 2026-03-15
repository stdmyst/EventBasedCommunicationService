using System.Reflection;
using EventBasedCommunication.Core;
using EventBasedCommunication.Core.Implementation;
using EventBasedCommunication.Core.Models;

var settings = ConfigurationHelpers.GetSettings<AppSettings>(Assembly.GetExecutingAssembly());
var eventBus = new EventService(settings.RabbitMqHostname);

await eventBus.Receive(exchange: "hello", args);