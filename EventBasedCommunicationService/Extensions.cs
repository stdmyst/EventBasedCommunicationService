using System.Reflection;
using EventBasedCommunicationService.Abstraction;
using EventBasedCommunicationService.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventBasedCommunicationService;

public static class Extensions
{
    public static IServiceCollection AddEventBasedCommunicationService(this IServiceCollection services, Assembly[] assemblies)
    {
        services.RegisterEventHandlers(assemblies);
        
        var eventResolver = new EventResolver(assemblies);
        services.AddSingleton(eventResolver);
        
        services.AddSingleton<IPublisher, EventService>();
        services.AddSingleton<ISubscriber, EventService>();
        
        return services;
    }

    private static IServiceCollection RegisterEventHandlers(this IServiceCollection services, Assembly[] assemblies)
    {
        var types = assemblies.SelectMany(a => a.GetTypes());
        
        foreach (var type in types)
        {
            var handlerInterfaces = type.GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));

            foreach (var handlerInterface in handlerInterfaces) 
                services.AddTransient(handlerInterface, type);
        }
        
        return services;
    }
}