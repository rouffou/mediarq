using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace Mediarq.Samples.Commands;

public record CreateUserCommand(string Name) : ICommand<Result<Guid>>;
