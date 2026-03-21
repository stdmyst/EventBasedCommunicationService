using System.Reflection;
using EventBasedCommunicationService.Abstraction;
using EventBasedCommunicationService.Models;

namespace EventBasedCommunicationService.Implementation;

internal class EventResolver
{
    private readonly Dictionary<string, HashSet<Type>> _events = new();
    private readonly Dictionary<Type, HashSet<Type>> _handlers = new();
    
    public string[] RoutingKeys => _events.Keys.ToArray();
    
    public EventResolver(Assembly[] assembliesToScan)
    {
        var types = assembliesToScan.SelectMany(assembly => assembly.GetTypes())
            .ToArray();
        
        MapSubscribers(types);
        
        MapHandlers(types);
    }

    public Type[] GetSubscribers(string routingKey) 
        => _events.TryGetValue(routingKey, out var types) ? types.ToArray() : [];
    
    public Type[] GetHandlers(Type subscriberType) 
        => _handlers.TryGetValue(subscriberType, out var handlers) ? handlers.ToArray() : [];
    
    private void MapSubscribers(Type[] types)
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
                var eventType = handlerInterface.GenericTypeArguments[0];
                
                if (!_handlers.ContainsKey(eventType))
                    _handlers.TryAdd(eventType, []);
                
                _handlers[eventType].Add(handlerInterface);
            }
        }
    }
}