namespace Mediarq.Core.Common.Pipeline;

/// <summary>
/// Optional interface that a <see cref="IPipelineBehavior{TRequest, TResponse}"/> can implement to
/// control its position in the pipeline. Behaviors with a lower <see cref="Order"/> run first (outermost);
/// behaviors that do not implement this interface run after those that do, in registration order.
/// </summary>
public interface IOrderBehavior
{
    /// <summary>
    /// Gets the relative order of the behavior. Lower values run earlier in the pipeline.
    /// </summary>
    int Order { get; }
}
