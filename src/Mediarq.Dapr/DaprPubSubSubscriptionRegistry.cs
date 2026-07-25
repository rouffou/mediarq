namespace Mediarq.Dapr;

/// <summary>
/// Collects the <see cref="DaprSubscription"/> entries registered by every
/// <see cref="DaprPubSubEndpointRouteBuilderExtensions.MapDaprPubSubSubscription{TNotification}"/> call,
/// so <see cref="DaprPubSubEndpointRouteBuilderExtensions.MapDaprPubSubSubscribeEndpoint"/> can serve them
/// all from the single well-known <c>/dapr/subscribe</c> endpoint the Dapr sidecar queries at startup.
/// </summary>
public sealed class DaprPubSubSubscriptionRegistry
{
    private readonly List<DaprSubscription> _subscriptions = [];

    /// <summary>The subscriptions registered so far.</summary>
    public IReadOnlyList<DaprSubscription> Subscriptions => _subscriptions;

    /// <summary>Adds a subscription.</summary>
    /// <param name="subscription">The subscription to add.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="subscription"/> is <see langword="null"/>.</exception>
    public void Add(DaprSubscription subscription)
    {
        ArgumentNullException.ThrowIfNull(subscription);
        _subscriptions.Add(subscription);
    }
}
