using System.Collections.Concurrent;

namespace Mediarq.WebApiTemplate1.Features.Todos;

public sealed class Todo
{
    public required Guid Id { get; init; }
    public required string Title { get; set; }
    public bool IsComplete { get; set; }
}

public interface ITodoStore
{
    Todo Add(Todo todo);
    Todo? Get(Guid id);
    IReadOnlyList<Todo> GetAll();
    bool Remove(Guid id);
}

/// <summary>In-memory, process-lifetime store. Replace with a real persistence layer in production.</summary>
public sealed class InMemoryTodoStore : ITodoStore
{
    private readonly ConcurrentDictionary<Guid, Todo> _todos = new();

    public Todo Add(Todo todo)
    {
        _todos[todo.Id] = todo;
        return todo;
    }

    public Todo? Get(Guid id) => _todos.GetValueOrDefault(id);

    public IReadOnlyList<Todo> GetAll() => _todos.Values.OrderBy(t => t.Title).ToList();

    public bool Remove(Guid id) => _todos.TryRemove(id, out _);
}
