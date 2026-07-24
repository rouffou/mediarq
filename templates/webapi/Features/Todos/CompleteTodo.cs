using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace Mediarq.WebApiTemplate1.Features.Todos;

public sealed record CompleteTodoCommand(Guid Id) : ICommand<Result>;

public sealed class CompleteTodoHandler(ITodoStore store) : ICommandHandler<CompleteTodoCommand, Result>
{
    public Task<Result> Handle(CompleteTodoCommand request, CancellationToken cancellationToken = default)
    {
        var todo = store.Get(request.Id);
        if (todo is null)
        {
            return Task.FromResult(Result.Failure(ResultError.NotFound("Todo.NotFound", $"Todo {request.Id} was not found.")));
        }

        todo.IsComplete = true;
        return Task.FromResult(Result.Success());
    }
}
