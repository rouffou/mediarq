namespace Mediarq.Outbox;

/// <summary>Options controlling the <see cref="OutboxProcessor{TContext}"/>.</summary>
public sealed class OutboxOptions
{
    /// <summary>Gets or sets how often the processor polls for pending messages. Default: 10 seconds.</summary>
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>Gets or sets the maximum number of messages published per poll. Default: 50.</summary>
    public int BatchSize { get; set; } = 50;

    /// <summary>
    /// Gets or sets how many publish attempts a message gets before it is left alone (dead-lettered). A
    /// message that reaches this count stays in the table with its last <see cref="OutboxMessage.Error"/>
    /// and is no longer picked up, so a poison message cannot fill every batch. Reset
    /// <see cref="OutboxMessage.Attempts"/> to retry it. Default: 10.
    /// </summary>
    public int MaxAttempts { get; set; } = 10;
}
