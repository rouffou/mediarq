using Mediarq.Core.Common.Requests.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mediarq.Grpc;

/// <summary>
/// Extension methods that register the publish and subscribe sides of gRPC cross-service notifications.
/// </summary>
public static class GrpcServiceCollectionExtensions
{
    /// <summary>
    /// Forwards a specific notification type to its remote service's Mediarq gRPC notification endpoint
    /// (<see cref="IGrpcNotificationEvent.ServiceAddress"/>) when it is published through Mediarq.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to forward.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqGrpcPublisher<TNotification>(this IServiceCollection services)
        where TNotification : class, IGrpcNotificationEvent
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<GrpcChannelCache>();
        services.AddScoped<INotificationHandler<TNotification>, GrpcNotificationForwarder<TNotification>>();
        return services;
    }

    /// <summary>
    /// Registers the gRPC hosting infrastructure and the <see cref="GrpcNotificationSubscriptionRegistry"/>
    /// that <see cref="GrpcEndpointRouteBuilderExtensions.MapMediarqGrpcNotificationService"/> and
    /// <see cref="GrpcEndpointRouteBuilderExtensions.MapMediarqGrpcSubscription{TNotification}"/> share.
    /// Call once before mapping any gRPC subscription.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqGrpcSubscriptions(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddGrpc();
        services.AddSingleton<GrpcNotificationSubscriptionRegistry>();
        return services;
    }
}
