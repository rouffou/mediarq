using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace Mediarq.App.Features;

/// <summary>The command. Replace <c>Name</c> with the data your command needs.</summary>
public sealed record SampleCommand(string Name) : ICommand<Result<string>>;
