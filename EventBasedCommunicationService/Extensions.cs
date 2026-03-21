using System.Reflection;
using EventBasedCommunicationService.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace EventBasedCommunicationService;

public static class Extensions
{
    public static IServiceCollection RegisterEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes();
        
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