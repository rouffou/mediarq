namespace Mediarq.Core.Common.Requests.Abstraction;

public interface IRequestHandler<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}
