namespace Mediarq.Outbox;

/// <summary>
/// A persisted notification awaiting reliable publication. Rows are written in the same transaction as
/// your business data and published later by the <see cref="OutboxProcessor{TContext}"/>.
/// </summary>
public sealed class OutboxMessage
{
    /// <summary>Gets or sets the unique identifier of the message.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the assembly-qualified CLR type name of the serialized notification.</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Gets or sets the JSON payload of the notification.</summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>Gets or sets when the message was enqueued (UTC).</summary>
    public DateTime OccurredOnUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets when the message was successfully published (UTC), or <see langword="null"/> while pending.</summary>
    public DateTime? ProcessedOnUtc { get; set; }

    /// <summary>Gets or sets how many publish attempts have been made.</summary>
    public int Attempts { get; set; }

    /// <summary>Gets or sets the last error recorded while attempting to publish, if any.</summary>
    public string? Error { get; set; }
}
