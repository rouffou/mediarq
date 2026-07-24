using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;

namespace Mediarq.WebApiTemplate1.Features.Todos;

/// <summary>
/// Minimal-API endpoints. Each one dispatches a Mediarq request and maps the <c>Result</c> to an HTTP
/// response via <c>ToHttpResultAsync()</c> — success becomes 200/204, failure becomes RFC 7807
/// ProblemDetails with a status derived from the <c>ResultError</c> category.
/// </summary>
public static class TodoEndpoints
{
    public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/todos").WithTags("Todos");

        // Create — runs the FluentValidation validator via the pipeline before the handler executes.
        group.MapPost("/", (CreateTodoCommand command, ISender sender)
            => sender.Send(command).ToHttpResultAsync());

        group.MapGet("/{id:guid}", (Guid id, ISender sender)
            => sender.Send(new GetTodoByIdQuery(id)).ToHttpResultAsync());

        group.MapGet("/", (ISender sender)
            => sender.Send(new ListTodosQuery()).ToHttpResultAsync());

        group.MapPost("/{id:guid}/complete", (Guid id, ISender sender)
            => sender.Send(new CompleteTodoCommand(id)).ToHttpResultAsync());

        group.MapDelete("/{id:guid}", (Guid id, ISender sender)
            => sender.Send(new DeleteTodoCommand(id)).ToHttpResultAsync());

        return app;
    }
}
