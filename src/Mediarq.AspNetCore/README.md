# Mediarq.AspNetCore

Map a Mediarq `Result` / `Result<T>` straight to an ASP.NET Core response — `200`/`Ok` on success, or
RFC 7807 `ProblemDetails` on failure with a status derived from the `ResultError` category.

```bash
dotnet add package Mediarq.AspNetCore
```

## Usage

No registration needed — just call the extension methods on the `Result` returned by `Send`.

**Minimal API:**
```csharp
app.MapPost("/orders", (CreateOrder cmd, ISender sender) => sender.Send(cmd).ToHttpResultAsync());
app.MapGet("/orders/{id:guid}", (Guid id, ISender sender) => sender.Send(new GetOrder(id)).ToHttpResultAsync());
```

**MVC controller:**
```csharp
[HttpPost]
public Task<IActionResult> Create(CreateOrder cmd) => mediator.Send(cmd).ToActionResultAsync();
```

Status mapping: `NotFound` → 404, `Validation` → 400, `Conflict` → 409, `Unauthorized` → 401,
`Forbidden` → 403, otherwise 400/500 as appropriate. A validation failure renders the property errors as
a ProblemDetails `errors` dictionary.

## Automatic endpoint mapping — `app.MapMediarq()`

Skip the pass-through endpoint entirely: annotate the command/query itself, and map every annotated type
in one call.

```csharp
[MediarqPost("/orders")]
public record CreateOrder(string Customer) : ICommand<Result<Guid>>;

[MediarqGet("/orders/{id}")]
public record GetOrder(Guid Id) : IQuery<Result<OrderDto>>;

[MediarqDelete("/orders/{id}")]
public record DeleteOrder(Guid Id) : ICommand; // no result -> 204 on success
```
```csharp
app.MapMediarq(typeof(CreateOrder).Assembly);
```

- `[MediarqGet]`/`[MediarqDelete]` — no request body; the request's members are bound individually from
  the route/query string (`[AsParameters]`), so `GetOrder(Guid Id)` above binds `Id` from `{id}`.
- `[MediarqPost]`/`[MediarqPut]`/`[MediarqPatch]` — the whole request is bound from the JSON body.
- The response is converted the same way `ToHttpResultAsync()` does above; a no-result `ICommand` maps a
  successful dispatch to `204 No Content`. Any other response type throws `InvalidOperationException` at
  startup — annotating a type that isn't `Result`/`Result<T>`/a no-result `ICommand` is a configuration
  mistake, not something to fail silently on.
- Returns a `RouteGroupBuilder`, so shared conventions apply to every mapped endpoint at once:
  `app.MapMediarq(...).RequireAuthorization().WithTags("orders");`.

## Learn more

- [Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
  [Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
