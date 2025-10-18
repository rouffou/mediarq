using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Samples.Models;

namespace Mediarq.Samples.Commands;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<Guid>>
{
    internal static readonly List<User> _users = new();
    public Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Name
        };

        _users.Add(user);

        return Task.FromResult(result: Result.Success(user.Id));
    }
}
