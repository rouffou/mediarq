# Mediarq.Authorization

ASP.NET Core policy-based authorization as a pipeline behavior: mark a command/query as
`IAuthorizedRequest`, and it is checked before its handler runs — no authenticated user short-circuits
with a 401, a failed policy short-circuits with a 403, both as a typed `Result` failure your handler
never has to think about.

```bash
dotnet add package Mediarq.Authorization
```

## Usage

```csharp
builder.Services.AddAuthorization(); // ASP.NET Core's own registration
builder.Services.AddHttpContextAccessor();
builder.Services.AddMediarqAuthorization();
```

```csharp
public record DeleteOrder(Guid OrderId) : ICommand, IAuthorizedRequest
{
    public string? PolicyName => "OrdersAdmin";
}
```

- `PolicyName` set → evaluated via `IAuthorizationService.AuthorizeAsync(user, request, policyName)`, so a
  custom `IAuthorizationHandler` can inspect the request itself (e.g. checking `OrderId` ownership), not
  just the policy name.
- `PolicyName` set to `null` → only requires an authenticated user, no specific policy.
- Not authenticated → `Result.Failure(ResultError.Unauthorized(...))` (maps to HTTP 401 via
  `Mediarq.AspNetCore`).
- Authenticated but the policy fails → `Result.Failure(ResultError.Forbidden(...))` (HTTP 403).
- Requests that don't implement `IAuthorizedRequest` pass straight through — this behavior costs nothing
  for them (`IConditionalPipelineBehavior.IsActive` is `false` for their closed type, so the pipeline
  never even resolves it into the chain).

## Response type

The handler's response type must be `Result` or `Result<T>` — the behavior needs some way to represent an
authorization failure as a value. `Result` is handled without reflection; `Result<T>` uses a
one-time-per-`T` reflection fallback (cached afterwards), so it is not on the Native AOT-safe path — the
same trade-off the core `ValidationBehavior` makes for its own `Result<T>` fallback.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
