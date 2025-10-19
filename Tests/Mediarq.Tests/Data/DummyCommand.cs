using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using System.Windows.Input;

namespace Mediarq.Tests.Data;

public record TestCommand(string Name) : ICommand<Result>;
public record TestCommandWithValue(string Name) : ICommand<Result<Guid>>;
public record TestCommandWithVReturnUnsupported(string Name) : ICommand<string>;
