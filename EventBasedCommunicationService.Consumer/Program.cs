using System.Reflection;
using EventBasedCommunicationService;
using EventBasedCommunicationService.Abstraction;
using EventBasedCommunicationService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var assembly = Assembly.GetExecutingAssembly();
var settings = ConfigurationHelpers.GetSettings<EventServiceSettings>(assembly);

var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging(builder => builder.AddConsole());
serviceCollection.AddEventBasedCommunicationService([assembly]);
serviceCollection.AddSingleton(Options.Create(settings));

var services = serviceCollection.BuildServiceProvider();

var eventBus = services.GetService<ISubscriber>() ?? throw new NullReferenceException("ISubscriber service not found.");

var cancellationTokenSource = new CancellationTokenSource();
await eventBus.Subscribe(exchange: "event-based-communication", cancellationTokenSource.Token);
