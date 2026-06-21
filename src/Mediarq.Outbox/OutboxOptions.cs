namespace Mediarq.Outbox;

/// <summary>Options controlling the <see cref="OutboxProcessor{TContext}"/>.</summary>
public sealed class OutboxOptions
{
    /// <summary>Gets or sets how often the processor polls for pending messages. Default: 10 seconds.</summary>
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>Gets or sets the maximum number of messages published per poll. Default: 50.</summary>
    public int BatchSize { get; set; } = 50;
}
