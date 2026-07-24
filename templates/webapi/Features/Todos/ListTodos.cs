using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace Mediarq.WebApiTemplate1.Features.Todos;

public sealed record ListTodosQuery : IQuery<Result<IReadOnlyList<TodoDto>>>;

public sealed class ListTodosHandler(ITodoStore store) : IQueryHandler<ListTodosQuery, Result<IReadOnlyList<TodoDto>>>
{
    public Task<Result<IReadOnlyList<TodoDto>>> Handle(ListTodosQuery request, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TodoDto> todos = store.GetAll().Select(TodoDto.From).ToList();
        return Task.FromResult(Result.Success(todos));
    }
}
