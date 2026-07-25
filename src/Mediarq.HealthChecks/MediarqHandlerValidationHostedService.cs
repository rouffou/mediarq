using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace Mediarq.HealthChecks;

/// <summary>
/// Runs <see cref="MediarqHandlerRegistrationValidator"/> once during host startup and throws if any
/// command/query does not resolve to exactly one handler, so a misconfiguration fails the app before
/// it starts accepting traffic instead of surfacing at the first dispatch.
/// </summary>
internal sealed class MediarqHandlerValidationHostedService(IServiceProvider serviceProvider, Assembly[] assemblies) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var issues = MediarqHandlerRegistrationValidator.Validate(serviceProvider, assemblies);

        if (issues.Count > 0)
        {
            throw new InvalidOperationException(
                "Mediarq handler registration validation failed at startup:" + Environment.NewLine +
                string.Join(Environment.NewLine, issues.Select(i => $" - {i}")));
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
