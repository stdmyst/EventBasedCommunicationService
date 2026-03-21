using System.Reflection;
using EventBasedCommunicationService;
using EventBasedCommunicationService.Implementation;
using EventBasedCommunicationService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var assembly = Assembly.GetExecutingAssembly();
var settings = ConfigurationHelpers.GetSettings<AppSettings>(assembly);

var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging(builder => builder.AddConsole());
serviceCollection.RegisterEventHandlers(assembly);
var services = serviceCollection.BuildServiceProvider();

var eventBus = new EventService(settings.RabbitMqHostname, assembly, services);

var cancellationTokenSource = new CancellationTokenSource();
await eventBus.Subscribe(exchange: "event-based-communication", cancellationTokenSource.Token);
