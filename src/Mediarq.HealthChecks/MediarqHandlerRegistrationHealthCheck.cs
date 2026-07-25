using System.Reflection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Mediarq.HealthChecks;

/// <summary>
/// Reports unhealthy when any command/query closed type does not resolve to exactly one
/// <c>IRequestHandler&lt;TRequest, TResponse&gt;</c> registration. See <see cref="MediarqHandlerRegistrationValidator"/>.
/// </summary>
public sealed class MediarqHandlerRegistrationHealthCheck(IServiceProvider serviceProvider, Assembly[] assemblies) : IHealthCheck
{
    /// <inheritdoc />
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var issues = MediarqHandlerRegistrationValidator.Validate(serviceProvider, assemblies);

        if (issues.Count == 0)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Every command/query has exactly one registered handler."));
        }

        var status = context.Registration.FailureStatus;
        var description = string.Join(" ", issues.Select(i => i.ToString()));
        var data = issues.ToDictionary(
            i => i.RequestType.FullName ?? i.RequestType.Name,
            object (i) => i.HandlerCount);

        return Task.FromResult(new HealthCheckResult(status, description, data: data));
    }
}
