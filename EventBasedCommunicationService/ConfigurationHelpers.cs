using System.Reflection;
using EventBasedCommunicationService.Models;
using Microsoft.Extensions.Configuration;

namespace EventBasedCommunicationService;

public static class ConfigurationHelpers
{
    public static TSettings GetSettings<TSettings>(Assembly assembly) 
        where TSettings : ISettings
    {
        var assemblyPath = Path.GetDirectoryName(assembly.Location) 
                           ?? throw new Exception("Assembly location can not be resolved.");
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(assemblyPath)
            .AddJsonFile("appsettings.json", true, true)
            .Build();

        var settings = configuration.Get<TSettings>() 
                       ?? throw new Exception($"{typeof(TSettings)} settings not found.");

        return settings;
    }
}