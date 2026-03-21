using System.Reflection;
using EventBasedCommunicationService.Abstraction;
using EventBasedCommunicationService.Models;

namespace EventBasedCommunicationService.Implementation;

public class EventScanner
{
    private readonly Dictionary<string, HashSet<Type>> _events = new();
    private readonly Dictionary<Type, HashSet<Func<IEvent, Task>>> _handlers = new();

    public string[] RoutingKeys => _events.Keys.ToArray();
    
    public EventScanner(Assembly assembly)
    {
        var types = assembly.GetTypes();
        ScanEvents(types);
        ScanHandlers(types);
    }

    public Type[] GetEvents(string routingKey) 
        => _events.TryGetValue(routingKey, out var types) ? types.ToArray() : [];
    
    public Func<IEvent, Task>[] GetHandlers(Type subscriberType) 
        => _handlers.TryGetValue(subscriberType, out var handlers) ? handlers.ToArray() : [];

    private void ScanEvents(Type[] types)
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
    
    private void ScanHandlers(Type[] types)
    {
        foreach (var type in types)
        {
            var handlerInterfaces = type.GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));

            foreach (var handlerInterface in handlerInterfaces)
            {
                var eventType = handlerInterface.GenericTypeArguments[0];
                
                var handlerInstance = Activator.CreateInstance(type);
                
                var handlerMethodInfo = handlerInterface.GetMethod(nameof(IEventHandler<>.Handle));
                if (handlerMethodInfo == null) continue;
                
                if (!_handlers.ContainsKey(eventType))
                    _handlers.TryAdd(eventType, []);

                _handlers[eventType].Add(async @event => await (Task)(handlerMethodInfo.Invoke(handlerInstance, parameters: [@event]) 
                                                                      ?? throw new InvalidOperationException()));
            }
        }
    }
}