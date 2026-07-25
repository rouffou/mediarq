using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Grpc;

/// <summary>
/// Endpoint mapping for the receiving half of gRPC cross-service notifications: the shared
/// <see cref="MediarqGrpcNotificationService"/>, and the per-notification-type subscription registration.
/// </summary>
public static class GrpcEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps the single shared gRPC service every subscribed notification type is multiplexed through.
    /// Requires <see cref="GrpcServiceCollectionExtensions.AddMediarqGrpcSubscriptions"/> to already be
    /// registered. Map exactly once.
    /// </summary>
    /// <param name="app">The endpoint route builder to map the service on.</param>
    /// <returns>The same route builder, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="app"/> is <see langword="null"/>.</exception>
    public static IEndpointRouteBuilder MapMediarqGrpcNotificationService(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapGrpcService<MediarqGrpcNotificationService>();
        return app;
    }

    /// <summary>
    /// Subscribes to <typeparamref name="TNotification"/>: registers it with the shared
    /// <see cref="GrpcNotificationSubscriptionRegistry"/> so a gRPC-delivered notification of this type is
    /// deserialized and republished through the real Mediarq pipeline via
    /// <see cref="Core.Mediators.IPublisher"/>. Requires
    /// <see cref="GrpcServiceCollectionExtensions.AddMediarqGrpcSubscriptions"/> to already be registered.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to receive.</typeparam>
    /// <param name="app">The endpoint route builder to register the subscription on.</param>
    /// <returns>The same route builder, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="app"/> is <see langword="null"/>.</exception>
    public static IEndpointRouteBuilder MapMediarqGrpcSubscription<TNotification>(this IEndpointRouteBuilder app)
        where TNotification : class, IGrpcNotificationEvent
    {
        ArgumentNullException.ThrowIfNull(app);

        var registry = app.ServiceProvider.GetRequiredService<GrpcNotificationSubscriptionRegistry>();
        registry.Add<TNotification>();
        return app;
    }
}
