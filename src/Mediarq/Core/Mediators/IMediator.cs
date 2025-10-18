using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Mediators;

public interface IMediator
{
    Task<TResponse> Send<TResponse>(ICommandOrQuery<TResponse> request, CancellationToken cancellationToken = default);
}
