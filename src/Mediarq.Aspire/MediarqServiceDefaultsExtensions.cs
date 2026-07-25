using System.Reflection;
using Mediarq.HealthChecks;
using Mediarq.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mediarq.Aspire;

/// <summary>
/// Wires Mediarq into an Aspire-generated <c>ServiceDefaults</c> project.
/// </summary>
/// <remarks>
/// This is additive, not a replacement: call <see cref="AddMediarqServiceDefaults"/> from inside your
/// own <c>ServiceDefaults</c> project's <c>AddServiceDefaults()</c> — alongside the OpenTelemetry,
/// service discovery and resilience setup the <c>dotnet new aspire-servicedefaults</c> template already
/// generates for you. This package does not reimplement any of that; it only adds Mediarq's own tracing/
/// metrics source and the handler-registration health check on top.
/// </remarks>
public static class MediarqServiceDefaultsExtensions
{
    /// <summary>
    /// Subscribes the tracer/meter providers to Mediarq's <c>Mediarq</c> activity source and meter (via
    /// <c>Mediarq.OpenTelemetry</c>), and registers <c>Mediarq.HealthChecks</c>' handler-registration
    /// check so a missing/ambiguous handler shows up in the Aspire dashboard instead of surfacing only as
    /// a runtime <c>HandlerNotFoundException</c>.
    /// </summary>
    /// <param name="builder">The host application builder — typically the one in your <c>ServiceDefaults</c> project.</param>
    /// <param name="assemblies">Assemblies to scan for commands/queries; defaults to the entry assembly when none are supplied.</param>
    /// <returns>The same builder, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is <see langword="null"/>.</exception>
    public static IHostApplicationBuilder AddMediarqServiceDefaults(this IHostApplicationBuilder builder, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddOpenTelemetry()
            .WithTracing(t => t.AddMediarqInstrumentation())
            .WithMetrics(m => m.AddMediarqInstrumentation());

        builder.Services.AddHealthChecks()
            .AddMediarqHandlerRegistrationCheck(assemblies: assemblies);

        return builder;
    }
}
