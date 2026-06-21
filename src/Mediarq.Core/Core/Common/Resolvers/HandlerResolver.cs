using System.Diagnostics.CodeAnalysis;

namespace Mediarq.Core.Common.Resolvers;

/// <summary>
/// Default <see cref="IHandlerResolver"/> implementation that delegates resolution to a
/// service-provider callback (typically <c>IServiceProvider.GetService</c>).
/// </summary>
public class HandlerResolver : IHandlerResolver
{
    private readonly Func<Type, object?> _resolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerResolver"/> class.
    /// </summary>
    /// <param name="resolver">The callback used to resolve services from the underlying container.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="resolver"/> is <see langword="null"/>.</exception>
    public HandlerResolver(Func<Type, object?> resolver)
    {
        ArgumentNullException.ThrowIfNull(resolver);

        _resolver = resolver;
    }

    /// <inheritdoc />
    public object? Resolve(Type handlerType) => _resolver(handlerType);

    /// <inheritdoc />
    public TService? Resolve<TService>() where TService : class => _resolver(typeof(TService)) as TService;

    /// <inheritdoc />
    [RequiresDynamicCode("Resolving by System.Type builds a closed IEnumerable<T> with MakeGenericType. Use ResolveAll<TService>() for an AOT-friendly path.")]
    public IEnumerable<object> ResolveAll(Type handlerType)
    {
        ArgumentNullException.ThrowIfNull(handlerType);

        var enumerableType = typeof(IEnumerable<>).MakeGenericType(handlerType);

        // The DI container is expected to resolve IEnumerable<THandler> (returning an empty
        // sequence when no handler is registered). We intentionally do not fall back to
        // resolving a single handler, which would silently mask a misconfigured registration.
        return _resolver(enumerableType) is IEnumerable<object> handlers ? handlers : [];
    }

    /// <inheritdoc />
    public IReadOnlyList<TService> ResolveAll<TService>()
    {
        // typeof(IEnumerable<TService>) is a closed constructed type known at the call site,
        // so no MakeGenericType is involved — this path is trimming/AOT friendly.
        if (_resolver(typeof(IEnumerable<TService>)) is not IEnumerable<TService> services)
        {
            return [];
        }

        return services as IReadOnlyList<TService> ?? [.. services];
    }
}
