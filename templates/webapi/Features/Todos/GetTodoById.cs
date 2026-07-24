using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace Mediarq.WebApiTemplate1.Features.Todos;

public sealed record TodoDto(Guid Id, string Title, bool IsComplete)
{
    public static TodoDto From(Todo todo) => new(todo.Id, todo.Title, todo.IsComplete);
}

public sealed record GetTodoByIdQuery(Guid Id) : IQuery<Result<TodoDto>>;

public sealed class GetTodoByIdHandler(ITodoStore store) : IQueryHandler<GetTodoByIdQuery, Result<TodoDto>>
{
    public Task<Result<TodoDto>> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken = default)
    {
        var todo = store.Get(request.Id);

        var result = todo is null
            ? Result.Failure<TodoDto>(ResultError.NotFound("Todo.NotFound", $"Todo {request.Id} was not found."))
            : Result.Success(TodoDto.From(todo));

        return Task.FromResult(result);
    }
}
