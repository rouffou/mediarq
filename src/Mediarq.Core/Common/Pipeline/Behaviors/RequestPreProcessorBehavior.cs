using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Processors;

namespace Mediarq.Core.Common.Pipeline.Behaviors;

/// <summary>
/// Pipeline behavior that runs every registered <see cref="IRequestPreProcessor{TRequest}"/> before
/// the request is handled.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
public sealed class RequestPreProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, IConditionalPipelineBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    private readonly IRequestPreProcessor<TRequest>[] _preProcessors;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPreProcessorBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="preProcessors">The pre-processors registered for this request type.</param>
    public RequestPreProcessorBehavior(IEnumerable<IRequestPreProcessor<TRequest>> preProcessors)
        => _preProcessors = preProcessors as IRequestPreProcessor<TRequest>[] ?? [.. preProcessors];

    /// <summary>Active only when at least one pre-processor is registered for this request type.</summary>
    public bool IsActive => _preProcessors.Length > 0;

    /// <inheritdoc />
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        foreach (var preProcessor in _preProcessors)
        {
            await preProcessor.Process(context.Request, cancellationToken).ConfigureAwait(false);
        }

        return await handle().ConfigureAwait(false);
    }
}
