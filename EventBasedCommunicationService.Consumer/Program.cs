using System.Reflection;
using EventBasedCommunicationService;
using EventBasedCommunicationService.Implementation;
using EventBasedCommunicationService.Models;

var settings = ConfigurationHelpers.GetSettings<AppSettings>(Assembly.GetExecutingAssembly());
var eventBus = new EventService(settings.RabbitMqHostname, Assembly.GetExecutingAssembly());

var cancellationTokenSource = new CancellationTokenSource();
await eventBus.Receive(exchange: "event-based-communication", cancellationTokenSource.Token);
