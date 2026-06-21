using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Mediarq.Caching;

/// <summary>
/// Default <see cref="IMediarqCacheSerializer"/> backed by <c>System.Text.Json</c>.
/// </summary>
/// <remarks>
/// This serializer is reflection-based and therefore not Native-AOT/trim safe. On AOT, register your own
/// <see cref="IMediarqCacheSerializer"/> (for example over a <c>JsonSerializerContext</c>).
/// </remarks>
public sealed class JsonMediarqCacheSerializer : IMediarqCacheSerializer
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    private const string ReflectionJustification =
        "The default cache serializer uses reflection-based System.Text.Json, which is not trimming/AOT " +
        "safe. AOT consumers should register a source-generated IMediarqCacheSerializer instead.";

    /// <inheritdoc />
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = ReflectionJustification)]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = ReflectionJustification)]
    public byte[] Serialize<TValue>(TValue value) => JsonSerializer.SerializeToUtf8Bytes(value, Options);

    /// <inheritdoc />
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = ReflectionJustification)]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = ReflectionJustification)]
    public TValue? Deserialize<TValue>(byte[] data) => JsonSerializer.Deserialize<TValue>(data, Options);
}
