namespace EventBasedCommunication.Core.Models;

public interface ISettings;

public record AppSettings : ISettings
{
    public required string RabbitMqHostname { get; init; }
}