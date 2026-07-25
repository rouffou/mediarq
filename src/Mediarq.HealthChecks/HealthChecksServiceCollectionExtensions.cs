using System.Reflection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mediarq.HealthChecks;

/// <summary>
/// Registration extensions for the Mediarq handler-registration health check.
/// </summary>
public static class HealthChecksServiceCollectionExtensions
{
    /// <summary>
    /// Registers an <see cref="IHealthCheck"/> that reports unhealthy when a command/query closed type
    /// does not resolve to exactly one registered handler.
    /// </summary>
    /// <param name="builder">The health checks builder, from <c>services.AddHealthChecks()</c>.</param>
    /// <param name="name">The health check's registered name.</param>
    /// <param name="failureStatus">The status reported when unhealthy; defaults to <see cref="HealthStatus.Unhealthy"/>.</param>
    /// <param name="tags">Optional tags for filtering which checks run for a given endpoint.</param>
    /// <param name="assemblies">Assemblies to scan for commands/queries; defaults to the entry assembly when none are supplied.</param>
    public static IHealthChecksBuilder AddMediarqHandlerRegistrationCheck(
        this IHealthChecksBuilder builder,
        string name = "mediarq_handlers",
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null,
        params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.Add(new HealthCheckRegistration(
            name,
            sp => new MediarqHandlerRegistrationHealthCheck(sp, assemblies),
            failureStatus,
            tags));
    }

    /// <summary>
    /// Registers a hosted service that runs the same handler-registration check once at host startup and
    /// throws <see cref="InvalidOperationException"/> if it finds any issue, failing the app fast instead
    /// of letting a misconfiguration surface as a runtime <c>HandlerNotFoundException</c> on first dispatch.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">Assemblies to scan for commands/queries; defaults to the entry assembly when none are supplied.</param>
    public static IServiceCollection AddMediarqHandlerValidationOnStartup(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService(sp => new MediarqHandlerValidationHostedService(sp, assemblies));
        return services;
    }
}
