using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Core.Mediators;

namespace Mediarq.Testing;

/// <summary>
/// Decorates a real <see cref="IMediator"/>, recording every dispatched request and published
/// notification while still delegating to the inner mediator — so handlers, pipeline behaviors and
/// validators all run exactly as they would in production; only the bookkeeping is added. Register via
/// <see cref="SpyMediatorServiceCollectionExtensions.AddMediarqSpy"/>, or construct directly around any
/// <see cref="IMediator"/> in a unit test.
/// </summary>
/// <param name="inner">The real mediator to delegate to after recording each call.</param>
public sealed class SpyMediator(IMediator inner) : IMediator
{
    private readonly IMediator _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    private readonly List<object> _sentRequests = [];
    private readonly List<object> _publishedNotifications = [];

    /// <summary>Every request passed to <see cref="Send{TResponse}"/>, <see cref="Send"/> or <see cref="CreateStream{TResponse}"/>, in dispatch order.</summary>
    public IReadOnlyList<object> SentRequests => _sentRequests;

    /// <summary>Every notification passed to <see cref="Publish{TNotification}"/>, in publish order.</summary>
    public IReadOnlyList<object> PublishedNotifications => _publishedNotifications;

    /// <summary>Clears the recorded requests and notifications, without affecting the inner mediator.</summary>
    public void Clear()
    {
        _sentRequests.Clear();
        _publishedNotifications.Clear();
    }

    /// <inheritdoc />
    public async Task<TResponse> Send<TResponse>(ICommandOrQuery<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        _sentRequests.Add(request);
        return await _inner.Send(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task Send(ICommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        _sentRequests.Add(request);
        await _inner.Send(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        _sentRequests.Add(request);
        return _inner.CreateStream(request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);
        _publishedNotifications.Add(notification);
        await _inner.Publish(notification, cancellationToken).ConfigureAwait(false);
    }
}
