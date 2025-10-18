using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Requests.Command;

public interface ICommand<TResponse> : ICommandOrQuery<TResponse>;
