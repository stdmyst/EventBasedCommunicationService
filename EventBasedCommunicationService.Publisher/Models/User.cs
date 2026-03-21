namespace EventBasedCommunicationService.Publisher.Models;

public record User
{
    public Guid Id { get; init; }
    
    public string? Username { get; init; }
}