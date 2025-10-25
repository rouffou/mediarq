
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
}
