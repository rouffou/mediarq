using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace Mediarq.Authorization.Tests.Fixtures;

public sealed record PlainCommand : ICommand<Result>;

public sealed record AuthorizedCommand(string? PolicyName) : ICommand<Result<string>>, IAuthorizedRequest;

public sealed record AuthorizedQuery(string? PolicyName) : IQuery<Result<int>>, IAuthorizedRequest;

public sealed record AuthorizedRawStringCommand(string? PolicyName) : ICommand<string>, IAuthorizedRequest;
