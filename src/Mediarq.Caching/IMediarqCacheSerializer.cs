namespace Mediarq.Caching;

/// <summary>
/// Serializes cached responses to and from bytes for the distributed cache. The default implementation
/// (<see cref="JsonMediarqCacheSerializer"/>) uses <c>System.Text.Json</c>; register your own (for
/// example a source-generated serializer) to control the format or to stay reflection-free on Native AOT.
/// </summary>
public interface IMediarqCacheSerializer
{
    /// <summary>Serializes <paramref name="value"/> to a byte array.</summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <returns>The serialized bytes.</returns>
    byte[] Serialize<TValue>(TValue value);

    /// <summary>Deserializes a value from <paramref name="data"/>.</summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="data">The bytes previously produced by <see cref="Serialize{TValue}(TValue)"/>.</param>
    /// <returns>The deserialized value, or <see langword="null"/> when it cannot be produced.</returns>
    TValue? Deserialize<TValue>(byte[] data);
}
