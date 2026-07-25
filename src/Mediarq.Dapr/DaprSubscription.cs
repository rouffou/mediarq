using System.Text.Json.Serialization;

namespace Mediarq.Dapr;

/// <summary>
/// One entry of the JSON array Dapr's programmatic subscription discovery expects from
/// <c>GET /dapr/subscribe</c>: which pub/sub component and topic to deliver to which local HTTP route.
/// </summary>
/// <param name="PubsubName">The Dapr pub/sub component name.</param>
/// <param name="Topic">The topic within <paramref name="PubsubName"/>.</param>
/// <param name="Route">The local HTTP route the Dapr sidecar POSTs matching messages to.</param>
public sealed record DaprSubscription(
    [property: JsonPropertyName("pubsubname")] string PubsubName,
    [property: JsonPropertyName("topic")] string Topic,
    [property: JsonPropertyName("route")] string Route);
