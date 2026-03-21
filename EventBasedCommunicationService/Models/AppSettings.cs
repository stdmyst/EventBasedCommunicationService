namespace EventBasedCommunicationService.Models;

public interface ISettings;

public record AppSettings : ISettings
{
    public required string RabbitMqHostname { get; init; }
}