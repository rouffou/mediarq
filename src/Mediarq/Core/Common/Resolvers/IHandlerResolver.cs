namespace Mediarq.Core.Common.Resolvers;

/// <summary>
/// Abstraction responsible for resolving handlers from the underlying DI container.
/// </summary>
public interface IHandlerResolver {

    /// <summary>
    /// Resolves the handler instance for the specified handler type.
    /// </summary>
    /// <param name="handlerType">Type of handler to resolve.</param>
    /// <returns>Resolbed handler instance, or null if not found</returns>
    object Resolve(Type handlerType);

    IEnumerable<object> ResolveAll(Type handlerType);
}
