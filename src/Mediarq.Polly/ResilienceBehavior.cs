using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using global::Polly;
using global::Polly.Registry;

namespace Mediarq.Polly;

/// <summary>
/// Pipeline behavior that executes <see cref="IResilientRequest"/> requests through the named Polly
/// <see cref="ResiliencePipeline"/> (retry, timeout, circuit breaker, ...). Other requests pass through.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
public sealed class ResilienceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    private readonly ResiliencePipelineProvider<string> _pipelineProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResilienceBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="pipelineProvider">The provider used to resolve named resilience pipelines.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="pipelineProvider"/> is <see langword="null"/>.</exception>
    public ResilienceBehavior(ResiliencePipelineProvider<string> pipelineProvider)
    {
        ArgumentNullException.ThrowIfNull(pipelineProvider);
        _pipelineProvider = pipelineProvider;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        if (context.Request is not IResilientRequest resilient)
        {
            return await handle().ConfigureAwait(false);
        }

        var pipeline = _pipelineProvider.GetPipeline(resilient.ResiliencePipelineName);

        return await pipeline
            .ExecuteAsync(async _ => await handle().ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }
}
