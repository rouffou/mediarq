namespace Mediarq.Core.Common.Resolvers;

/// <summary>
/// Abstraction responsible for resolving handlers from the underlying DI container.
/// </summary>
public interface IHandlerResolver {

    /// <summary>
    /// Resolves the handler instance for the specified handler type.
    /// </summary>
    /// <param name="handlerType">Type of handler to resolve.</param>
    /// <returns>The resolved handler instance, or <see langword="null"/> if not found.</returns>
    object? Resolve(Type handlerType);

    /// <summary>
    /// Resolves every handler registered for the specified handler type.
    /// </summary>
    /// <param name="handlerType">The handler type to resolve.</param>
    /// <returns>The resolved handler instances; an empty sequence when none are registered.</returns>
    IEnumerable<object> ResolveAll(Type handlerType);
}
