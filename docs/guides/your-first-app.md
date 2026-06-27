# Your first app

Build a tiny but real ASP.NET Core API with Mediarq, from an empty folder to a running endpoint with a
command, a query, validation and a notification. ~10 minutes. Copy each block in order.

> Prefer to read finished code? See [Samples/Mediarq.Samples.WebApi](https://github.com/rouffou/mediarq/tree/main/Samples/Mediarq.Samples.WebApi).
> Want a console instead of a web app? See [Samples/Mediarq.Samples.Quickstart](https://github.com/rouffou/mediarq/tree/main/Samples/Mediarq.Samples.Quickstart).

## 1. Create the project

```bash
dotnet new web -n TodoApi
cd TodoApi
dotnet add package Mediarq                     # core + lightweight extensions (incl. AspNetCore, FluentValidation)
dotnet add package FluentValidation.DependencyInjectionExtensions
```

## 2. A place to store data

Keep it simple — an in-memory store registered as a singleton (swap for a repository/`DbContext` later).

```csharp
// Store.cs
using System.Collections.Concurrent;

public sealed record Todo(Guid Id, string Title, bool Done);

public interface ITodoStore
{
    void Add(Todo todo);
    Todo? Find(Guid id);
    IReadOnlyCollection<Todo> All();
}

public sealed class InMemoryTodoStore : ITodoStore
{
    private readonly ConcurrentDictionary<Guid, Todo> _todos = new();
    public void Add(Todo todo) => _todos[todo.Id] = todo;
    public Todo? Find(Guid id) => _todos.TryGetValue(id, out var t) ? t : null;
    public IReadOnlyCollection<Todo> All() => _todos.Values.ToArray();
}
```

## 3. A command (writes) and its handler

```csharp
// CreateTodo.cs
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;

public record CreateTodo(string Title) : ICommand<Result<Guid>>;

public class CreateTodoHandler(ITodoStore store, IPublisher publisher) : ICommandHandler<CreateTodo, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTodo request, CancellationToken ct = default)
    {
        var todo = new Todo(Guid.NewGuid(), request.Title, Done: false);
        store.Add(todo);
        await publisher.Publish(new TodoCreated(todo.Id, todo.Title), ct); // step 6
        return Result.Success(todo.Id);
    }
}
```

## 4. A query (reads) and its handler

```csharp
// GetTodo.cs
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

public record GetTodo(Guid Id) : IQuery<Result<Todo>>;

public class GetTodoHandler(ITodoStore store) : IQueryHandler<GetTodo, Result<Todo>>
{
    public Task<Result<Todo>> Handle(GetTodo request, CancellationToken ct = default)
    {
        var todo = store.Find(request.Id);
        return Task.FromResult(todo is null
            ? Result.Failure<Todo>(ResultError.NotFound("Todo.NotFound", $"Todo {request.Id} not found."))
            : Result.Success(todo));
    }
}
```

## 5. Validation

```csharp
// CreateTodoValidator.cs
using FluentValidation;

public class CreateTodoValidator : AbstractValidator<CreateTodo>
{
    public CreateTodoValidator() => RuleFor(x => x.Title).NotEmpty().MaximumLength(120);
}
```

## 6. A notification (something happened)

```csharp
// TodoCreated.cs
using Mediarq.Core.Common.Requests.Notifications;

public record TodoCreated(Guid Id, string Title) : INotification;

public class LogTodoCreated(ILogger<LogTodoCreated> logger) : INotificationHandler<TodoCreated>
{
    public Task Handle(TodoCreated n, CancellationToken ct = default)
    {
        logger.LogInformation("Todo created: {Title} ({Id})", n.Title, n.Id);
        return Task.CompletedTask;
    }
}
```

## 7. Wire it up and expose endpoints

```csharp
// Program.cs
using FluentValidation;
using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Mediarq.Extensions;
using Mediarq.FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITodoStore, InMemoryTodoStore>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTodoValidator>();

// ⚠️ Register the validation adapter BEFORE AddMediarq so the scan wires the ValidationBehavior.
builder.Services.AddMediarqFluentValidation();
builder.Services.AddMediarq(isHttp: false, typeof(Program).Assembly);

var app = builder.Build();

// Result -> HTTP: success becomes 200/Ok, failure becomes a ProblemDetails (404, 400, …).
app.MapPost("/todos", (CreateTodo cmd, ISender sender) => sender.Send(cmd).ToHttpResultAsync());
app.MapGet("/todos/{id:guid}", (Guid id, ISender sender) => sender.Send(new GetTodo(id)).ToHttpResultAsync());

app.Run();
```

The one line to remember is the ⚠️ comment: with the assembly-scan registration, register your
validators (here, the FluentValidation adapter) **before** `AddMediarq`. If you forget, validation
silently does nothing — see [Troubleshooting](troubleshooting.md#my-validation-never-runs-no-error-the-handler-just-runs).

## 8. Run

```bash
dotnet run
```

```bash
# create one
curl -s -X POST http://localhost:5000/todos -H 'Content-Type: application/json' -d '{"title":"Buy milk"}'
# -> "a1b2c3d4-..."  (and a "Todo created" line in the console, from the notification handler)

# read it back
curl -s http://localhost:5000/todos/a1b2c3d4-...
# -> {"id":"...","title":"Buy milk","done":false}

# invalid -> 400 ProblemDetails
curl -s -X POST http://localhost:5000/todos -H 'Content-Type: application/json' -d '{"title":""}'
# -> {"type":"...","title":"One or more validation errors occurred.","status":400,"errors":{...}}
```

That's a complete vertical slice: command → handler → notification, query → handler, validation, and
`Result` → HTTP.

## Where next

- [Wiring extensions](wiring-extensions.md) — add caching, idempotency, EF Core, outbox, resilience, …
- [Testing](testing.md) — unit- and integration-test what you just built.
- [Concepts](concepts.md) — when to reach for a command vs a query vs a notification.
- Switch to the **reflection-free** registration for trimming/AOT: replace
  `AddMediarq(...)` with `AddMediarqCore(...).AddMediarqHandlers()` (see [Native AOT](native-aot.md)).
