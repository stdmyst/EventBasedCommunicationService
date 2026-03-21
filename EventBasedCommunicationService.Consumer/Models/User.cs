namespace EventBasedCommunicationService.Consumer.Models;

public record User
{
    public Guid Id { get; init; }
    
    public string? Username { get; init; }
}