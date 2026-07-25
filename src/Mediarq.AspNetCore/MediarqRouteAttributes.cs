namespace Mediarq.AspNetCore;

/// <summary>
/// Maps a command/query to <c>GET {Pattern}</c> via <see cref="MediarqEndpointRouteBuilderExtensions.MapMediarq"/>.
/// Route/query values are bound onto the request's members individually (<c>[AsParameters]</c>), by name —
/// there is no request body on a GET.
/// </summary>
/// <param name="pattern">The route pattern, e.g. <c>"/orders/{id}"</c>.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class MediarqGetAttribute(string pattern) : Attribute
{
    /// <summary>The route pattern.</summary>
    public string Pattern { get; } = pattern ?? throw new ArgumentNullException(nameof(pattern));
}

/// <summary>
/// Maps a command/query to <c>POST {Pattern}</c> via <see cref="MediarqEndpointRouteBuilderExtensions.MapMediarq"/>.
/// The request is bound from the JSON body.
/// </summary>
/// <param name="pattern">The route pattern, e.g. <c>"/orders"</c>.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class MediarqPostAttribute(string pattern) : Attribute
{
    /// <summary>The route pattern.</summary>
    public string Pattern { get; } = pattern ?? throw new ArgumentNullException(nameof(pattern));
}

/// <summary>
/// Maps a command/query to <c>PUT {Pattern}</c> via <see cref="MediarqEndpointRouteBuilderExtensions.MapMediarq"/>.
/// The request is bound from the JSON body.
/// </summary>
/// <param name="pattern">The route pattern, e.g. <c>"/orders/{id}"</c>.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class MediarqPutAttribute(string pattern) : Attribute
{
    /// <summary>The route pattern.</summary>
    public string Pattern { get; } = pattern ?? throw new ArgumentNullException(nameof(pattern));
}

/// <summary>
/// Maps a command/query to <c>PATCH {Pattern}</c> via <see cref="MediarqEndpointRouteBuilderExtensions.MapMediarq"/>.
/// The request is bound from the JSON body.
/// </summary>
/// <param name="pattern">The route pattern, e.g. <c>"/orders/{id}"</c>.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class MediarqPatchAttribute(string pattern) : Attribute
{
    /// <summary>The route pattern.</summary>
    public string Pattern { get; } = pattern ?? throw new ArgumentNullException(nameof(pattern));
}

/// <summary>
/// Maps a command/query to <c>DELETE {Pattern}</c> via <see cref="MediarqEndpointRouteBuilderExtensions.MapMediarq"/>.
/// Route/query values are bound onto the request's members individually (<c>[AsParameters]</c>), by name —
/// there is no request body on a DELETE.
/// </summary>
/// <param name="pattern">The route pattern, e.g. <c>"/orders/{id}"</c>.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class MediarqDeleteAttribute(string pattern) : Attribute
{
    /// <summary>The route pattern.</summary>
    public string Pattern { get; } = pattern ?? throw new ArgumentNullException(nameof(pattern));
}
