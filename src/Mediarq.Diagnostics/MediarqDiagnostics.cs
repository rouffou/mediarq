using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Mediarq.Diagnostics;

/// <summary>
/// Central diagnostics primitives for Mediarq: the <see cref="ActivitySource"/> used for tracing and
/// the metric instruments. Subscribe to the <see cref="SourceName"/> source/meter from OpenTelemetry
/// or <c>System.Diagnostics</c> listeners.
/// </summary>
public static class MediarqDiagnostics
{
    /// <summary>The name shared by the Mediarq <see cref="ActivitySource"/> and <see cref="System.Diagnostics.Metrics.Meter"/>.</summary>
    public const string SourceName = "Mediarq";

    /// <summary>The activity source that emits one activity per dispatched request.</summary>
    public static readonly ActivitySource ActivitySource = new(SourceName);

    private static readonly Meter Meter = new(SourceName);
    private static readonly Counter<long> RequestCounter = Meter.CreateCounter<long>("mediarq.requests.count", "{request}", "Number of requests dispatched through Mediarq.");
    private static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>("mediarq.requests.duration", "ms", "Duration of Mediarq request handling.");

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
}
