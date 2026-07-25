using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace Mediarq.RateLimiting.Tests.Fixtures;

public sealed record LimitedCommand(string PolicyName, string? PartitionKey) : ICommand<Result<string>>, IRateLimitedRequest;

public sealed record PlainCommand : ICommand<Result<string>>;
