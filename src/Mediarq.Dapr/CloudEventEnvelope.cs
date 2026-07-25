using System.Text.Json.Serialization;

namespace Mediarq.Dapr;

/// <summary>
/// The subset of a Dapr-delivered CloudEvents 1.0 envelope this package cares about: the actual payload,
/// under <c>data</c>. Deliberately not a full CloudEvents implementation — every other envelope field
/// (<c>id</c>, <c>source</c>, <c>type</c>, <c>specversion</c>, ...) is ignored rather than modeled.
/// </summary>
/// <typeparam name="TData">The payload type.</typeparam>
public sealed class CloudEventEnvelope<TData>
{
    /// <summary>The event payload, as published via <c>DaprClient.PublishEventAsync</c>.</summary>
    [JsonPropertyName("data")]
    public TData? Data { get; set; }
}
