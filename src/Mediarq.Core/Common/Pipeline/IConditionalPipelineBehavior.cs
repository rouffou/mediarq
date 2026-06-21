namespace Mediarq.Core.Common.Pipeline;

/// <summary>
/// Optional interface a <see cref="IPipelineBehavior{TRequest, TResponse}"/> can implement to declare
/// that it has nothing to do for the current request type. The <see cref="PipelineExecutor"/> skips
/// behaviors whose <see cref="IsActive"/> is <see langword="false"/>, so they add neither an async
/// frame nor a delegate to the pipeline chain.
/// </summary>
/// <remarks>
/// This lets the built-in "plumbing" behaviors (validation, pre/post processors, exception handling)
/// stay registered yet cost nothing when no validator, processor or exception handler exists for the
/// request — keeping the hot path close to a bare handler invocation.
/// </remarks>
public interface IConditionalPipelineBehavior
{
    /// <summary>
    /// Gets a value indicating whether this behavior should take part in the pipeline for the current
    /// request type. When <see langword="false"/>, the executor omits it entirely.
    /// </summary>
    bool IsActive { get; }
}
