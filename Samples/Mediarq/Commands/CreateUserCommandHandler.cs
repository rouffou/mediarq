using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.Samples.Models;
using Mediarq.Samples.Notifications;

namespace Mediarq.Samples.Commands;

public class CreateUserCommandHandler(IPublisher publisher) : ICommandHandler<CreateUserCommand, Result<Guid>>
{
    internal static readonly List<User> _users = new();

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Name
        };

        _users.Add(user);

        // Publish a notification: every registered handler (audit, welcome email, ...) runs.
        await publisher.Publish(new UserCreated(user.Id, user.UserName), cancellationToken);

        return Result.Success(user.Id);
    }
}
