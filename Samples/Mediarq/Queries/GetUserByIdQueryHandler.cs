using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Samples.Commands;
using Mediarq.Samples.Models;

namespace Mediarq.Samples.Queries;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Result<User>>
{
    private static readonly List<User> _users = CreateUserCommandHandler._users;
    public Task<Result<User>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(u => u.Id == request.Id);

        if (user is null)
        {
            return Task.FromResult(Result.Failure<User>(Error.NotFound("User.NotFound", $"User with Id {request.Id} not found.")));
        }

        return Task.FromResult(Result.Success(user));
    }
}
