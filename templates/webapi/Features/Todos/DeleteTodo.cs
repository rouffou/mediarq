using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace Mediarq.WebApiTemplate1.Features.Todos;

public sealed record DeleteTodoCommand(Guid Id) : ICommand<Result>;

public sealed class DeleteTodoHandler(ITodoStore store) : ICommandHandler<DeleteTodoCommand, Result>
{
    public Task<Result> Handle(DeleteTodoCommand request, CancellationToken cancellationToken = default)
    {
        var result = store.Remove(request.Id)
            ? Result.Success()
            : Result.Failure(ResultError.NotFound("Todo.NotFound", $"Todo {request.Id} was not found."));

        return Task.FromResult(result);
    }
}
