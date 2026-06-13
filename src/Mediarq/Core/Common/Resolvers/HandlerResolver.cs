

namespace Mediarq.Core.Common.Resolvers;

public class HandlerResolver : IHandlerResolver {

    private readonly Func<Type, object> _resolver;

    public HandlerResolver(Func<Type, object> resolver) {
        ArgumentNullException.ThrowIfNull(resolver);

        _resolver = resolver;
    }

    public object Resolve(Type handlerType) {
        return _resolver(handlerType);
    }

    public IEnumerable<object> ResolveAll(Type handlerType) {
        ArgumentNullException.ThrowIfNull(handlerType);

        var enumerableType = typeof(IEnumerable<>).MakeGenericType(handlerType);

        // The DI container is expected to resolve IEnumerable<THandler> (returning an empty
        // sequence when no handler is registered). We intentionally do not fall back to
        // resolving a single handler, which would silently mask a misconfigured registration.
        return _resolver(enumerableType) is IEnumerable<object> handlers ? handlers : [];
    }
}
