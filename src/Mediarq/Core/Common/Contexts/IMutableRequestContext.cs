using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Contexts;

public interface IMutableRequestContext<TRequest, TResponse> : IRequestContext<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    void AddItem(string key, object value);
}
