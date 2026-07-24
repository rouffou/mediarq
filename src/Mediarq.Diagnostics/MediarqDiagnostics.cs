using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Mediarq.Diagnostics;

/// <summary>
/// Central diagnostics primitives for Mediarq: the <see cref="ActivitySource"/> used for tracing and
/// the metric instruments. Subscribe to the <see cref="SourceName"/> source/meter from OpenTelemetry
/// or <c>System.Diagnostics</c> listeners.
/// </summary>
/// <remarks>
/// Alongside the original Mediarq-specific instruments/tags (kept as-is — renaming them would break any
/// dashboard already built against v1.x), every activity and the
/// <see cref="MessagingOperationDuration"/> metric also carry the subset of the
/// <see href="https://opentelemetry.io/docs/specs/semconv/messaging/">OpenTelemetry semantic conventions
/// for messaging</see> that apply to an in-process mediator: <c>messaging.system</c>,
/// <c>messaging.operation.type</c>, <c>messaging.destination.name</c>, <c>messaging.message.id</c>,
/// <c>messaging.batch.message_count</c> and the general <c>error.type</c> attribute. See
/// <c>docs/guides/observability-dashboard.md</c> for a ready-to-use dashboard built on these.
/// </remarks>
public static class MediarqDiagnostics
{
    /// <summary>The name shared by the Mediarq <see cref="ActivitySource"/> and <see cref="System.Diagnostics.Metrics.Meter"/>.</summary>
    public const string SourceName = "Mediarq";

    /// <summary>The <c>messaging.system</c> semantic convention value identifying Mediarq's in-process dispatch.</summary>
    public const string MessagingSystem = "mediarq";

    /// <summary>The activity source that emits one activity per dispatched request.</summary>
    public static readonly ActivitySource ActivitySource = new(SourceName);

    private static readonly Meter Meter = new(SourceName);
    private static readonly Counter<long> RequestCounter = Meter.CreateCounter<long>("mediarq.requests.count", "{request}", "Number of requests dispatched through Mediarq.");
    private static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>("mediarq.requests.duration", "ms", "Duration of Mediarq request handling.");

    /// <summary>
    /// The OpenTelemetry messaging semantic convention's <c>messaging.client.operation.duration</c>
    /// histogram (seconds, per the spec), recorded alongside <c>mediarq.requests.duration</c>.
    /// </summary>
    private static readonly Histogram<double> MessagingOperationDuration = Meter.CreateHistogram<double>(
        "messaging.client.operation.duration", "s", "Duration of a Mediarq dispatch, tagged per OpenTelemetry messaging semantic conventions.");

    internal static void Record(string requestName, TimeSpan elapsed, bool succeeded)
    {
        var tags = new TagList
        {
            { "mediarq.request", requestName },
            { "mediarq.outcome", succeeded ? "success" : "failure" },
        };

        RequestCounter.Add(1, tags);
        RequestDuration.Record(elapsed.TotalMilliseconds, tags);
    }

    /// <summary>
    /// Sets the OpenTelemetry messaging semantic convention tags on <paramref name="activity"/> and
    /// records <see cref="MessagingOperationDuration"/>, in addition to (not instead of) the original
    /// <c>mediarq.*</c> tags/metric a caller already set via <see cref="Record"/>.
    /// </summary>
    /// <param name="activity">The current activity, or <see langword="null"/> when nothing is sampling it.</param>
    /// <param name="operationType">
    /// The <c>messaging.operation.type</c> value: <c>"process"</c> for a request/query dispatch or a
    /// stream, <c>"publish"</c> for a notification fan-out.
    /// </param>
    /// <param name="destinationName">The <c>messaging.destination.name</c> — the request/notification type name.</param>
    /// <param name="elapsed">The measured duration.</param>
    /// <param name="messageId">The <c>messaging.message.id</c>, when one is available (e.g. the request id).</param>
    /// <param name="batchMessageCount">
    /// The <c>messaging.batch.message_count</c>, for operations that fan out to more than one
    /// destination (a notification's handler count, a stream's item count).
    /// </param>
    /// <param name="errorType">The <c>error.type</c> — the exception's type name, when the operation failed.</param>
    internal static void RecordMessagingOperation(
        Activity? activity,
        string operationType,
        string destinationName,
        TimeSpan elapsed,
        string? messageId = null,
        long? batchMessageCount = null,
        string? errorType = null)
    {
        if (activity is not null)
        {
            activity.SetTag("messaging.system", MessagingSystem);
            activity.SetTag("messaging.operation.type", operationType);
            activity.SetTag("messaging.destination.name", destinationName);

            if (messageId is not null)
            {
                activity.SetTag("messaging.message.id", messageId);
            }

            if (batchMessageCount is not null)
            {
                activity.SetTag("messaging.batch.message_count", batchMessageCount.Value);
            }

            if (errorType is not null)
            {
                activity.SetTag("error.type", errorType);
            }
        }

        var tags = new TagList
        {
            { "messaging.system", MessagingSystem },
            { "messaging.operation.type", operationType },
            { "messaging.destination.name", destinationName },
        };

        if (errorType is not null)
        {
            tags.Add("error.type", errorType);
        }

        MessagingOperationDuration.Record(elapsed.TotalSeconds, tags);
    }
}
