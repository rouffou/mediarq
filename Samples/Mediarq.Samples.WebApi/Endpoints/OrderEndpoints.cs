using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Mediarq.Samples.WebApi.Domain;
using Mediarq.Samples.WebApi.Features.Orders;

namespace Mediarq.Samples.WebApi.Endpoints;

/// <summary>Request body for the "add note" endpoint (the id comes from the route).</summary>
public sealed record AddOrderNoteBody(string Note);

/// <summary>
/// Minimal-API endpoints. Each one dispatches a Mediarq request and maps the <c>Result</c> to an HTTP
/// response via <c>ToHttpResultAsync()</c> — success becomes 200/Ok, failure becomes RFC 7807
/// ProblemDetails with a status derived from the <c>ResultError</c> category.
/// </summary>
public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders").WithTags("Orders");

        // Create — runs FluentValidation, persists via the unit of work, enqueues an outbox event.
        group.MapPost("/", (CreateOrderCommand command, ISender sender)
            => sender.Send(command).ToHttpResultAsync());

        // Get — memoized by the caching behavior (second identical call skips the DB read).
        group.MapGet("/{id:guid}", (Guid id, ISender sender)
            => sender.Send(new GetOrderByIdQuery(id)).ToHttpResultAsync());

        // Confirm — idempotent: pass the same Idempotency-Key header to replay instead of re-running.
        group.MapPost("/{id:guid}/confirm", (Guid id, HttpRequest http, ISender sender) =>
        {
            var key = http.Headers["Idempotency-Key"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            return sender.Send(new ConfirmOrderCommand(id, key)).ToHttpResultAsync();
        });

        // Add note — validated with DataAnnotations (try an empty note to see a 400 ProblemDetails).
        group.MapPost("/{id:guid}/note", (Guid id, AddOrderNoteBody body, ISender sender)
            => sender.Send(new AddOrderNoteCommand { OrderId = id, Note = body.Note }).ToHttpResultAsync());

        // Quote — executed through the Polly resilience pipeline (retries the flaky pricing service).
        group.MapGet("/{id:guid}/quote", (Guid id, ISender sender)
            => sender.Send(new QuotePriceQuery(id)).ToHttpResultAsync());

        // Stream — returns orders as an IAsyncEnumerable (streamed JSON array).
        group.MapGet("/stream", (ISender sender)
            => sender.CreateStream(new StreamOrdersRequest()));

        return app;
    }
}
