using Mediarq.Diagnostics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Mediarq.OpenTelemetry;

/// <summary>
/// OpenTelemetry registration helpers that subscribe the tracer and meter providers to the
/// Mediarq activity source and metrics (see <see cref="MediarqDiagnostics.SourceName"/>).
/// </summary>
public static class MediarqInstrumentationExtensions
{
    /// <summary>
    /// Subscribes the tracer provider to the Mediarq <see cref="System.Diagnostics.ActivitySource"/>,
    /// so the per-request activities emitted by Mediarq.Diagnostics are exported.
    /// </summary>
    /// <param name="builder">The tracer provider builder to configure.</param>
    /// <returns>The same builder, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is <see langword="null"/>.</exception>
    public static TracerProviderBuilder AddMediarqInstrumentation(this TracerProviderBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddSource(MediarqDiagnostics.SourceName);
    }

    /// <summary>
    /// Subscribes the meter provider to the Mediarq <see cref="System.Diagnostics.Metrics.Meter"/>,
    /// so the request count and duration instruments emitted by Mediarq.Diagnostics are exported.
    /// </summary>
    /// <param name="builder">The meter provider builder to configure.</param>
    /// <returns>The same builder, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is <see langword="null"/>.</exception>
    public static MeterProviderBuilder AddMediarqInstrumentation(this MeterProviderBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.AddMeter(MediarqDiagnostics.SourceName);
    }
}
