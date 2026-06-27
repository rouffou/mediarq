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
otherwise 400/500 as appropriate. A validation failure renders the property errors as a ProblemDetails
`errors` dictionary.

## Learn more

- [Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
  [Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
