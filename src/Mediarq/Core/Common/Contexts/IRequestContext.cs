using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Contexts;

public interface IRequestContext<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    Guid RequestId { get; }
    string UserId { get; }
    DateTime StartedAt { get; }
    DateTime FinishedAt { get; set; }
    TRequest Request { get; }
    CancellationToken CancellationToken { get; }
    IReadOnlyDictionary<string, object> Items { get; }
}
