using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Resolvers;

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
            return new ValueTask<TResponse>(handler.Handle(request, cancellationToken));
        }

        var behaviors = handlerResolver.ResolveAll<IPipelineBehavior<TRequest, TResponse>>();

        if (behaviors.Count == 0)
        {
            behaviorRegistrationCache?.MarkKnownEmpty<TRequest, TResponse>();
            return new ValueTask<TResponse>(handler.Handle(request, cancellationToken));
        }

        var active = SelectActive<TRequest, TResponse>(behaviors, out var activeCount);

        // No active behavior: skip the request context allocation and the delegate entirely, invoking
        // the handler directly — the hot path costs no more than a bare handler call.
        if (active is null)
        {
            return new ValueTask<TResponse>(handler.Handle(request, cancellationToken));
        }

        // A behavior will observe the context, so create it now (lazily, only when actually needed).
        RequestContext<TRequest, TResponse> context = requestContextFactory.Create<TRequest, TResponse>(request, cancellationToken);
        return new ValueTask<TResponse>(Run(active, activeCount, context, () => handler.Handle(request, cancellationToken), cancellationToken));
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
