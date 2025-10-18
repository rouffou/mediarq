using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Requests.Query;

public interface IQuery<TResponse> : ICommandOrQuery<TResponse>;
