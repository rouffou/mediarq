using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Mediarq.Core.Common.Requests.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Dapr;

/// <summary>
/// Extension methods that register <see cref="DaprPubSubNotificationForwarder{TNotification}"/> so
/// Mediarq notifications are forwarded to a Dapr pub/sub component, and the registry backing the
/// <c>/dapr/subscribe</c> discovery endpoint.
/// </summary>
/// <remarks>
/// These only register the forwarder/registry. Register the Dapr client itself separately
/// (<c>services.AddDaprClient()</c>, from the <c>Dapr.AspNetCore</c>/<c>Dapr.Client</c> packages).
/// </remarks>
public static class DaprServiceCollectionExtensions
{
    /// <summary>
    /// Forwards a specific notification type to its Dapr pub/sub component/topic when it is published
    /// through Mediarq.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to forward.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqDaprPubSub<TNotification>(this IServiceCollection services)
        where TNotification : class, IDaprPubSubEvent
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<INotificationHandler<TNotification>, DaprPubSubNotificationForwarder<TNotification>>();
        return services;
    }

    /// <summary>
    /// Forwards every <see cref="IDaprPubSubEvent"/> discovered in the given assemblies to its Dapr
    /// pub/sub component/topic when published through Mediarq.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="assemblies">The assemblies to scan for <see cref="IDaprPubSubEvent"/> types.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="assemblies"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// This overload uses reflection. For trimming/AOT, prefer the generic
    /// <see cref="AddMediarqDaprPubSub{TNotification}(IServiceCollection)"/> per event type.
    /// </remarks>
    [RequiresUnreferencedCode("Scans assemblies for IDaprPubSubEvent types via reflection. Use the generic AddMediarqDaprPubSub<TNotification>() per event for a trimming/AOT-friendly registration.")]
    [RequiresDynamicCode("Builds closed generic handler/forwarder types with MakeGenericType. Use the generic AddMediarqDaprPubSub<TNotification>() per event for an AOT-friendly registration.")]
    public static IServiceCollection AddMediarqDaprPubSub(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        foreach (var assembly in assemblies)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            foreach (var type in assembly.GetTypes())
            {
                if (type is { IsAbstract: false, IsClass: true, IsGenericTypeDefinition: false } &&
                    typeof(IDaprPubSubEvent).IsAssignableFrom(type))
                {
                    var serviceType = typeof(INotificationHandler<>).MakeGenericType(type);
                    var implementationType = typeof(DaprPubSubNotificationForwarder<>).MakeGenericType(type);
                    services.AddScoped(serviceType, implementationType);
                }
            }
        }

        return services;
    }

    /// <summary>
    /// Registers the <see cref="DaprPubSubSubscriptionRegistry"/> that
    /// <see cref="DaprPubSubEndpointRouteBuilderExtensions.MapDaprPubSubSubscription{TNotification}"/> and
    /// <see cref="DaprPubSubEndpointRouteBuilderExtensions.MapDaprPubSubSubscribeEndpoint"/> share.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqDaprPubSubSubscriptions(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<DaprPubSubSubscriptionRegistry>();
        return services;
    }
}
