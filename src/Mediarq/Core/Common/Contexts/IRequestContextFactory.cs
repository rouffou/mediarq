using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Contexts;

public interface IRequestContextFactory
{
    object Create<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : ICommandOrQuery<TResponse>;
}
