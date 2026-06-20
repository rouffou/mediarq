using Mediarq.Core.Common.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Diagnostics;

/// <summary>
/// Extension methods that register the Mediarq diagnostics behavior.
/// </summary>
public static class DiagnosticsServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <see cref="DiagnosticsBehavior{TRequest, TResponse}"/> so every request emits an
    /// <see cref="System.Diagnostics.Activity"/> and metrics under the <see cref="MediarqDiagnostics.SourceName"/> source.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Subscribe with OpenTelemetry (<c>AddSource("Mediarq")</c> / <c>AddMeter("Mediarq")</c>) or a
    /// <c>System.Diagnostics</c> listener to collect the data. Call after <c>AddMediarq</c>/<c>AddMediarqCore</c>.
    /// </remarks>
    public static IServiceCollection AddMediarqDiagnostics(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DiagnosticsBehavior<,>));
        return services;
    }
}
