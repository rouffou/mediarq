using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Dapr;

/// <summary>
/// Minimal-API endpoints for the receiving half of Dapr pub/sub: a webhook per subscribed notification
/// type, and the <c>/dapr/subscribe</c> discovery endpoint the Dapr sidecar queries at startup to learn
/// which topics to deliver where.
/// </summary>
public static class DaprPubSubEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps a webhook that receives <typeparamref name="TNotification"/> from its Dapr pub/sub
    /// component/topic and republishes it through the real Mediarq pipeline via
    /// <see cref="IPublisher"/>, and registers the route with the shared
    /// <see cref="DaprPubSubSubscriptionRegistry"/> so <see cref="MapDaprPubSubSubscribeEndpoint"/> can
    /// advertise it. Requires <c>AddMediarqDaprPubSubSubscriptions()</c> to already be registered.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to receive.</typeparam>
    /// <param name="app">The endpoint route builder to map the webhook on.</param>
    /// <param name="route">The local route to receive on; defaults to <c>/dapr/pubsub/{TNotification name}</c>.</param>
    /// <returns>The same route builder, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="app"/> is <see langword="null"/>.</exception>
    public static IEndpointRouteBuilder MapDaprPubSubSubscription<TNotification>(this IEndpointRouteBuilder app, string? route = null)
        where TNotification : class, IDaprPubSubEvent
    {
        ArgumentNullException.ThrowIfNull(app);

        route ??= $"/dapr/pubsub/{typeof(TNotification).Name}";

        var registry = app.ServiceProvider.GetRequiredService<DaprPubSubSubscriptionRegistry>();
        registry.Add(new DaprSubscription(TNotification.PubsubName, TNotification.Topic, route));

        app.MapPost(route, async (CloudEventEnvelope<TNotification> envelope, IPublisher publisher, CancellationToken cancellationToken) =>
        {
            if (envelope.Data is null)
            {
                return Results.BadRequest();
            }

            await publisher.Publish(envelope.Data, cancellationToken).ConfigureAwait(false);
            return Results.Ok();
        });

        return app;
    }

    /// <summary>
    /// Maps Dapr's well-known programmatic subscription discovery endpoint (<c>GET /dapr/subscribe</c>),
    /// serving every subscription registered so far via <see cref="MapDaprPubSubSubscription{TNotification}"/>.
    /// Map this <em>after</em> every <see cref="MapDaprPubSubSubscription{TNotification}"/> call.
    /// </summary>
    /// <param name="app">The endpoint route builder to map the discovery endpoint on.</param>
    /// <returns>The same route builder, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="app"/> is <see langword="null"/>.</exception>
    public static IEndpointRouteBuilder MapDaprPubSubSubscribeEndpoint(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var registry = app.ServiceProvider.GetRequiredService<DaprPubSubSubscriptionRegistry>();
        app.MapGet("/dapr/subscribe", () => Results.Json(registry.Subscriptions));

        return app;
    }
}
