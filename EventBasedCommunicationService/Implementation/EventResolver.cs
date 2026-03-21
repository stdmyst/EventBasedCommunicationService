using System.Reflection;
using EventBasedCommunicationService.Abstraction;
using EventBasedCommunicationService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventBasedCommunicationService.Implementation;

public class EventResolver
{
    private readonly Dictionary<string, HashSet<Type>> _events = new();
    private readonly Dictionary<Type, HashSet<Func<IEvent, Task>>> _handlers = new();

    private readonly IServiceProvider _services;
    private readonly ILogger<EventResolver> _logger;

    public string[] RoutingKeys => _events.Keys.ToArray();
    
    public EventResolver(Assembly assembly, IServiceProvider services)
    {
        _services = services;
        _logger = services.GetService<ILogger<EventResolver>>() 
                 ?? throw new NullReferenceException("No logger defined.");;
        
        var types = assembly.GetTypes();
        
        MapEvents(types);
        
        MapHandlers(types);
    }

    public Type[] GetEvents(string routingKey) 
        => _events.TryGetValue(routingKey, out var types) ? types.ToArray() : [];
    
    public Func<IEvent, Task>[] GetHandlers(Type subscriberType) 
        => _handlers.TryGetValue(subscriberType, out var handlers) ? handlers.ToArray() : [];

    private void MapEvents(Type[] types)
    {
        var subscribers = types.Where(t 
            => t.GetCustomAttribute<EventSubscribeAttribute>() != null 
               && t.GetInterface(nameof(IEvent)) != null);

        foreach (var subscriber in subscribers)
        {
            if (Activator.CreateInstance(subscriber) is IEvent @event)
            {
                if (!_events.ContainsKey(@event.RoutingKey))
                    _events.TryAdd(@event.RoutingKey, []);

                _events[@event.RoutingKey].Add(subscriber);
            }
        }
    }

    private void MapHandlers(Type[] types)
    {
        foreach (var type in types)
        {
            var handlerInterfaces = type.GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));

            foreach (var handlerInterface in handlerInterfaces)
            {
                var handler = _services.GetService(handlerInterface);
                var handlerMethodInfo = handler?.GetType().GetMethod(nameof(IEventHandler<>.Handle));
                if (handler == null || handlerMethodInfo == null) 
                    continue;
                
                var eventType = handlerInterface.GenericTypeArguments[0];
                
                if (!_handlers.ContainsKey(eventType))
                    _handlers.TryAdd(eventType, []);

                _handlers[eventType].Add(async @event => await (Task)(handlerMethodInfo.Invoke(handler, parameters: [@event]) 
                                                                      ?? throw new InvalidOperationException()));
            }
        }
    }
}