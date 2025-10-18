using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Requests.Query;

public interface IQueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
}
