using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace Mediarq.Samples.Quickstart.Orders;

/// <summary>
/// A deliberately slow command that opts into a timeout by implementing <see cref="ITimeoutRequest"/>.
/// With <c>AddMediarqTimeout()</c> registered, exceeding <see cref="Timeout"/> throws a
/// <c>RequestTimeoutException</c> (a pessimistic timeout — the handler should also honor its token).
/// </summary>
public sealed record GenerateReportCommand : ICommand<Result<string>>, ITimeoutRequest
{
    public TimeSpan Timeout => TimeSpan.FromMilliseconds(150);
}

public sealed class GenerateReportCommandHandler : ICommandHandler<GenerateReportCommand, Result<string>>
{
    public async Task<Result<string>> Handle(GenerateReportCommand request, CancellationToken cancellationToken = default)
    {
        // Takes much longer than the 150 ms budget, so the TimeoutBehavior frees the caller first.
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        return Result.Success("report ready");
    }
}
