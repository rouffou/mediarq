namespace Mediarq.Core.Common.Requests.Streaming;

/// <summary>
/// Marker for a request that streams its response as an <see cref="IAsyncEnumerable{T}"/> of
/// <typeparamref name="TResponse"/> items, dispatched through <c>ISender.CreateStream</c>.
/// </summary>
/// <typeparam name="TResponse">The type of each streamed item.</typeparam>
public interface IStreamRequest<out TResponse>;
