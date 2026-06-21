using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Processors;

namespace Mediarq.Core.Common.Pipeline.Behaviors;

/// <summary>
/// Pipeline behavior that runs every registered <see cref="IRequestPostProcessor{TRequest, TResponse}"/>
/// after the request has been handled.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
public sealed class RequestPostProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, IConditionalPipelineBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    private readonly IRequestPostProcessor<TRequest, TResponse>[] _postProcessors;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPostProcessorBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="postProcessors">The post-processors registered for this request/response pair.</param>
    public RequestPostProcessorBehavior(IEnumerable<IRequestPostProcessor<TRequest, TResponse>> postProcessors)
        => _postProcessors = postProcessors as IRequestPostProcessor<TRequest, TResponse>[] ?? [.. postProcessors];

    /// <summary>Active only when at least one post-processor is registered for this request/response pair.</summary>
    public bool IsActive => _postProcessors.Length > 0;

    /// <inheritdoc />
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        var response = await handle().ConfigureAwait(false);

        foreach (var postProcessor in _postProcessors)
        {
            await postProcessor.Process(context.Request, response, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }
}
