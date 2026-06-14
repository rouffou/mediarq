using System.Diagnostics.CodeAnalysis;

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
    /// Resolves the handler instance for the specified service type, without any reflection.
    /// </summary>
    /// <typeparam name="TService">The service type to resolve.</typeparam>
    /// <returns>The resolved instance, or <see langword="null"/> if not registered.</returns>
    TService? Resolve<TService>() where TService : class;

    /// <summary>
    /// Resolves every handler registered for the specified handler type.
    /// </summary>
    /// <param name="handlerType">The handler type to resolve.</param>
    /// <returns>The resolved handler instances; an empty sequence when none are registered.</returns>
    /// <remarks>
    /// This overload builds a closed <c>IEnumerable&lt;T&gt;</c> via reflection. Prefer the
    /// generic <see cref="ResolveAll{TService}"/> overload, which is trimming/AOT friendly.
    /// </remarks>
    [RequiresDynamicCode("Resolving by System.Type builds a closed IEnumerable<T> with MakeGenericType. Use ResolveAll<TService>() for an AOT-friendly path.")]
    IEnumerable<object> ResolveAll(Type handlerType);

    /// <summary>
    /// Resolves every service registered for <typeparamref name="TService"/>, without any reflection.
    /// </summary>
    /// <typeparam name="TService">The service type to resolve.</typeparam>
    /// <returns>The resolved instances; an empty list when none are registered.</returns>
    IReadOnlyList<TService> ResolveAll<TService>();
}
