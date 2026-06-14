using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mediarq.Core.Common.Requests.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.MassTransit;

/// <summary>
/// Extension methods that register <see cref="MassTransitNotificationForwarder{TNotification}"/> so
/// Mediarq notifications are forwarded to a MassTransit bus (published out-of-process).
/// </summary>
/// <remarks>
/// These only register the forwarder. Configure the MassTransit bus itself separately (for example
/// <c>services.AddMassTransit(...)</c>), which provides the <see cref="global::MassTransit.IPublishEndpoint"/>.
/// </remarks>
public static class MassTransitServiceCollectionExtensions
{
    /// <summary>
    /// Forwards a specific notification type to the MassTransit bus when it is published through Mediarq.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to forward.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqMassTransitForwarding<TNotification>(this IServiceCollection services)
        where TNotification : class, INotification
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<INotificationHandler<TNotification>, MassTransitNotificationForwarder<TNotification>>();
        return services;
    }

    /// <summary>
    /// Forwards every <see cref="IIntegrationEvent"/> discovered in the given assemblies to the
    /// MassTransit bus when published through Mediarq.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="assemblies">The assemblies to scan for <see cref="IIntegrationEvent"/> types.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="assemblies"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// This overload uses reflection. For trimming/AOT, prefer the generic
    /// <see cref="AddMediarqMassTransitForwarding{TNotification}(IServiceCollection)"/> per event type.
    /// </remarks>
    [RequiresUnreferencedCode("Scans assemblies for IIntegrationEvent types via reflection. Use the generic AddMediarqMassTransitForwarding<TNotification>() per event for a trimming/AOT-friendly registration.")]
    [RequiresDynamicCode("Builds closed generic handler/forwarder types with MakeGenericType. Use the generic AddMediarqMassTransitForwarding<TNotification>() per event for an AOT-friendly registration.")]
    public static IServiceCollection AddMediarqMassTransitForwarding(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        foreach (var assembly in assemblies)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            foreach (var type in assembly.GetTypes())
            {
                if (type is { IsAbstract: false, IsClass: true, IsGenericTypeDefinition: false } &&
                    typeof(IIntegrationEvent).IsAssignableFrom(type))
                {
                    var serviceType = typeof(INotificationHandler<>).MakeGenericType(type);
                    var implementationType = typeof(MassTransitNotificationForwarder<>).MakeGenericType(type);
                    services.AddScoped(serviceType, implementationType);
                }
            }
        }

        return services;
    }
}
