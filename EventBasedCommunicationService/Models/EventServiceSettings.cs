namespace EventBasedCommunicationService.Models;

public interface ISettings;

public record EventServiceSettings : ISettings
{
    public required string RabbitMqHostname { get; init; }
    
    public int? TimeoutSeconds { get; init; }
}