using System.Threading;
using System.Threading.Tasks;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace Mediarq.App.Features;

/// <summary>Handles <see cref="SampleCommand"/> and returns a <see cref="Result{T}"/>.</summary>
public sealed class SampleCommandHandler : ICommandHandler<SampleCommand, Result<string>>
{
    public Task<Result<string>> Handle(SampleCommand request, CancellationToken cancellationToken = default)
    {
        // TODO: implement the command. Return Result.Failure(...) for expected failures.
        return Task.FromResult(Result.Success(request.Name));
    }
}
