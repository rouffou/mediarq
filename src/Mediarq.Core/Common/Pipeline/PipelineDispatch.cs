using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Resolvers;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;

namespace Mediarq.Core.Common.Pipeline;

/// <summary>
/// Shared behavior-pipeline logic used by the request dispatch wrapper and <see cref="PipelineExecutor"/>:
/// selecting the active behaviors (honoring <see cref="IConditionalPipelineBehavior"/> and
/// <see cref="IOrderBehavior"/>) and running the resulting chain around the handler. Kept allocation-free
/// on the common paths so the dispatch stays close to a bare handler call.
/// </summary>
internal static class PipelineDispatch
{
    /// <summary>
    /// Returns the ordered list of active behaviors, or <see langword="null"/> when none are active.
    /// The resolver's list is reused as-is when every behavior is active; otherwise the active ones are
    /// compacted into a fresh array. Ordering is applied only when a behavior opts into it.
    /// </summary>
    public static IReadOnlyList<IPipelineBehavior<TRequest, TResponse>>? SelectActive<TRequest, TResponse>(
        IReadOnlyList<IPipelineBehavior<TRequest, TResponse>> behaviors,
        out int activeCount)
        where TRequest : ICommandOrQuery<TResponse>
    {
        var count = behaviors.Count;
        activeCount = 0;
        var hasOrdering = false;
        for (var i = 0; i < count; i++)
        {
            var behavior = behaviors[i];
            if (behavior is IConditionalPipelineBehavior { IsActive: false })
            {
                continue;
            }

            activeCount++;
            if (behavior is IOrderBehavior)
            {
                hasOrdering = true;
            }
        }

        if (activeCount == 0)
        {
            return null;
        }

        IReadOnlyList<IPipelineBehavior<TRequest, TResponse>> active;
        if (activeCount == count)
        {
            active = behaviors;
        }
        else
        {
            var compacted = new IPipelineBehavior<TRequest, TResponse>[activeCount];
            var k = 0;
            for (var i = 0; i < count; i++)
            {
                var behavior = behaviors[i];
                if (behavior is IConditionalPipelineBehavior { IsActive: false })
                {
                    continue;
                }

                compacted[k++] = behavior;
            }

            active = compacted;
        }

        if (hasOrdering)
        {
            active = StableSortByOrder(active, activeCount);
        }

        return active;
    }

    /// <summary>
    /// Builds the chain from the innermost (handler) outwards, so the first behavior runs first.
    /// <paramref name="handlerDelegate"/> already closes over whatever cancellation token it needs — Run
    /// always invokes it as the tail with no further wrapping, so callers that already have the token in
    /// scope (the common case) pay for exactly one closure instead of two.
    /// </summary>
    public static Task<TResponse> Run<TRequest, TResponse>(
        IReadOnlyList<IPipelineBehavior<TRequest, TResponse>> active,
        int activeCount,
        RequestContext<TRequest, TResponse> context,
        Func<Task<TResponse>> handlerDelegate,
        CancellationToken cancellationToken)
        where TRequest : ICommandOrQuery<TResponse>
    {
        var next = handlerDelegate;
        for (var i = activeCount - 1; i >= 0; i--)
        {
            var behavior = active[i];
            var currentNext = next;
            next = () => behavior.Handle(context, currentNext, cancellationToken);
        }

        return next();
    }

    /// <summary>
    /// Shared cache-aware dispatch used by both <see cref="PipelineExecutor"/> and the source-generated
    /// request wrapper: skips <see cref="IHandlerResolver.ResolveAll{TService}"/> entirely once a closed
    /// <typeparamref name="TRequest"/>/<typeparamref name="TResponse"/> pair is known (via
    /// <paramref name="behaviorRegistrationCache"/>) to have zero registered behaviors, and returns a
    /// <see cref="ValueTask{TResponse}"/> that wraps the handler's own <see cref="Task{TResponse}"/> at no
    /// extra allocation cost — callers that must stay <see cref="Task{TResponse}"/>-returning for
    /// public-API compatibility call <see cref="ValueTask{TResponse}.AsTask"/>, which is itself
    /// allocation-free when the <see cref="ValueTask{TResponse}"/> already wraps a real
    /// <see cref="Task{TResponse}"/> (always true here).
    /// </summary>
    public static ValueTask<TResponse> ExecuteWithBehaviorCache<TRequest, TResponse>(
        IHandlerResolver handlerResolver,
        PipelineBehaviorRegistrationCache? behaviorRegistrationCache,
        TRequest request,
        IRequestHandler<TRequest, TResponse> handler,
        IRequestContextFactory requestContextFactory,
        CancellationToken cancellationToken)
        where TRequest : ICommandOrQuery<TResponse>
    {
        if (behaviorRegistrationCache?.IsKnownEmpty<TRequest, TResponse>() == true)
        {
            return WithCascadedNotifications(handler.Handle(request, cancellationToken), handlerResolver, cancellationToken);
        }

        var behaviors = handlerResolver.ResolveAll<IPipelineBehavior<TRequest, TResponse>>();

        if (behaviors.Count == 0)
        {
            behaviorRegistrationCache?.MarkKnownEmpty<TRequest, TResponse>();
            return WithCascadedNotifications(handler.Handle(request, cancellationToken), handlerResolver, cancellationToken);
        }

        var active = SelectActive<TRequest, TResponse>(behaviors, out var activeCount);

        // No active behavior: skip the request context allocation and the delegate entirely, invoking
        // the handler directly — the hot path costs no more than a bare handler call.
        if (active is null)
        {
            return WithCascadedNotifications(handler.Handle(request, cancellationToken), handlerResolver, cancellationToken);
        }

        // A behavior will observe the context, so create it now (lazily, only when actually needed).
        RequestContext<TRequest, TResponse> context = requestContextFactory.Create<TRequest, TResponse>(request, cancellationToken);
        return WithCascadedNotifications(
            Run(active, activeCount, context, () => handler.Handle(request, cancellationToken), cancellationToken),
            handlerResolver,
            cancellationToken);
    }

    // Publishes notifications a handler attached to a successful Result/Result<T> via
    // Result.WithNotifications(...), once the request has finished dispatching (after every behavior,
    // exception handler and post-processor has run) — never on a failed result or a thrown exception.
    // Gated on CascadeSupport<TResponse>.IsResultResponse (computed once per closed TResponse type) so
    // a response type unrelated to Result pays nothing extra: the original task is returned as-is, same
    // as before this feature existed. For a Result/Result<T> response that completed synchronously (the
    // common case for the built-in benchmarks and most handlers), the check itself is synchronous too —
    // the async continuation is only paid for when there is something to actually await or publish.
    private static ValueTask<TResponse> WithCascadedNotifications<TResponse>(
        Task<TResponse> responseTask, IHandlerResolver handlerResolver, CancellationToken cancellationToken)
    {
        if (!CascadeSupport<TResponse>.IsResultResponse)
        {
            return new ValueTask<TResponse>(responseTask);
        }

        if (responseTask.IsCompletedSuccessfully)
        {
            var response = responseTask.Result;
            return HasCascadedNotifications(response, out var notifications)
                ? new ValueTask<TResponse>(PublishThenReturnAsync(response, notifications, handlerResolver, cancellationToken))
                : new ValueTask<TResponse>(responseTask);
        }

        return new ValueTask<TResponse>(AwaitThenPublishAsync(responseTask, handlerResolver, cancellationToken));
    }

    private static bool HasCascadedNotifications<TResponse>(TResponse response, out IReadOnlyList<INotification> notifications)
    {
        if (response is Result { IsSuccess: true } result && result.CascadedNotifications.Count > 0)
        {
            notifications = result.CascadedNotifications;
            return true;
        }

        notifications = [];
        return false;
    }

    private static async Task<TResponse> PublishThenReturnAsync<TResponse>(
        TResponse response, IReadOnlyList<INotification> notifications, IHandlerResolver handlerResolver, CancellationToken cancellationToken)
    {
        await PublishAllAsync(notifications, handlerResolver, cancellationToken).ConfigureAwait(false);
        return response;
    }

    private static async Task<TResponse> AwaitThenPublishAsync<TResponse>(
        Task<TResponse> responseTask, IHandlerResolver handlerResolver, CancellationToken cancellationToken)
    {
        var response = await responseTask.ConfigureAwait(false);
        if (HasCascadedNotifications(response, out var notifications))
        {
            await PublishAllAsync(notifications, handlerResolver, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }

    // Cascaded notifications are published through the same resolved IPublisher (and therefore the same
    // registered INotificationPublisher -- Parallel/Sequential/AggregateException) as an explicit
    // IPublisher.Publish(...) call would use. Silently does nothing if IPublisher is not registered
    // (e.g. a hand-built IHandlerResolver in a test that never registered the core services).
    private static async Task PublishAllAsync(
        IReadOnlyList<INotification> notifications, IHandlerResolver handlerResolver, CancellationToken cancellationToken)
    {
        var publisher = handlerResolver.Resolve<IPublisher>();
        if (publisher is null)
        {
            return;
        }

        for (var i = 0; i < notifications.Count; i++)
        {
            await publisher.Publish(notifications[i], cancellationToken).ConfigureAwait(false);
        }
    }

    // Cached once per closed TResponse type (the standard EqualityComparer<T>.Default-style pattern),
    // so the "does this response type even support cascading" check costs nothing beyond the first call
    // for a given TResponse -- most response types are not Result-derived and never pay for this feature.
    private static class CascadeSupport<TResponse>
    {
        public static readonly bool IsResultResponse = typeof(Result).IsAssignableFrom(typeof(TResponse));
    }

    // Stable insertion sort by ascending Order (default int.MaxValue), copying into a fresh array so the
    // resolver's list is never mutated. Insertion sort is stable and allocation-free beyond that copy,
    // and behavior counts are small, so it is cheaper than LINQ OrderBy on this hot path.
    private static IPipelineBehavior<TRequest, TResponse>[] StableSortByOrder<TRequest, TResponse>(
        IReadOnlyList<IPipelineBehavior<TRequest, TResponse>> behaviors,
        int count)
        where TRequest : ICommandOrQuery<TResponse>
    {
        var sorted = new IPipelineBehavior<TRequest, TResponse>[count];
        for (var i = 0; i < count; i++)
        {
            sorted[i] = behaviors[i];
        }

        for (var i = 1; i < count; i++)
        {
            var current = sorted[i];
            var currentOrder = current is IOrderBehavior order ? order.Order : int.MaxValue;
            var j = i - 1;
            while (j >= 0 && OrderOf(sorted[j]) > currentOrder)
            {
                sorted[j + 1] = sorted[j];
                j--;
            }
            sorted[j + 1] = current;
        }

        return sorted;
    }

    private static int OrderOf<TRequest, TResponse>(IPipelineBehavior<TRequest, TResponse> behavior)
        where TRequest : ICommandOrQuery<TResponse>
        => behavior is IOrderBehavior order ? order.Order : int.MaxValue;
}
