using System.Linq;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Diagnostics;

/// <summary>
/// Extension methods that register the Mediarq diagnostics behaviors and notification decorator.
/// </summary>
public static class DiagnosticsServiceCollectionExtensions
{
    /// <summary>
    /// Adds Mediarq observability: an <see cref="System.Diagnostics.Activity"/> + metrics for every
    /// <c>Send</c> (<see cref="DiagnosticsBehavior{TRequest, TResponse}"/>), <c>CreateStream</c>
    /// (<see cref="StreamDiagnosticsBehavior{TRequest, TResponse}"/>) and <c>Publish</c>
    /// (<see cref="DiagnosticsNotificationPublisher"/>), under the <see cref="MediarqDiagnostics.SourceName"/> source.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Subscribe with OpenTelemetry (<c>AddSource("Mediarq")</c> / <c>AddMeter("Mediarq")</c>) or a
    /// <c>System.Diagnostics</c> listener to collect the data. Call after <c>AddMediarq</c>/<c>AddMediarqCore</c>
    /// so the notification publisher is already registered and can be decorated.
    /// </remarks>
    public static IServiceCollection AddMediarqDiagnostics(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DiagnosticsBehavior<,>));
        services.AddScoped(typeof(IStreamPipelineBehavior<,>), typeof(StreamDiagnosticsBehavior<,>));
        DecorateNotificationPublisher(services);
        return services;
    }

    // Wraps the registered INotificationPublisher with DiagnosticsNotificationPublisher, without
    // reflection: the concrete implementation is re-registered and resolved through the container.
    private static void DecorateNotificationPublisher(IServiceCollection services)
    {
        var descriptor = services.LastOrDefault(d => d.ServiceType == typeof(INotificationPublisher));
        if (descriptor is null)
        {
            return;
        }

        services.Remove(descriptor);

        if (descriptor.ImplementationInstance is INotificationPublisher instance)
        {
            services.Add(new ServiceDescriptor(
                typeof(INotificationPublisher),
                _ => new DiagnosticsNotificationPublisher(instance),
                descriptor.Lifetime));
        }
        else if (descriptor.ImplementationFactory is { } factory)
        {
            services.Add(new ServiceDescriptor(
                typeof(INotificationPublisher),
                sp => new DiagnosticsNotificationPublisher((INotificationPublisher)factory(sp)),
                descriptor.Lifetime));
        }
        else if (descriptor.ImplementationType is { } implementationType)
        {
            services.Add(new ServiceDescriptor(implementationType, implementationType, descriptor.Lifetime));
            services.Add(new ServiceDescriptor(
                typeof(INotificationPublisher),
                sp => new DiagnosticsNotificationPublisher((INotificationPublisher)sp.GetRequiredService(implementationType)),
                descriptor.Lifetime));
        }
    }
}
