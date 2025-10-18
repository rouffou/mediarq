namespace Mediarq.Samples.Models;

public record User
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = string.Empty;
}
