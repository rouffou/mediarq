# Mediarq.FluentValidation

Run your [FluentValidation](https://fluentvalidation.net) validators inside the Mediarq validation
pipeline, so invalid requests short-circuit with a failed `Result` before the handler runs.

```bash
dotnet add package Mediarq.FluentValidation
```

## Usage

```csharp
builder.Services.AddValidatorsFromAssemblyContaining<Program>(); // your AbstractValidator<T> classes
builder.Services.AddMediarqFluentValidation();
```

```csharp
public class CreateOrderValidator : AbstractValidator<CreateOrder>
{
    public CreateOrderValidator() => RuleFor(x => x.Customer).NotEmpty().MaximumLength(100);
}
```

An invalid request returns `Result.Failure(...)` carrying the property errors (a notification throws
`NotificationValidationException` instead, since it has no return value).

> ⚠️ **Registration order (scan core only).** With `AddMediarq(...)` (assembly scan), call
> `AddMediarqFluentValidation()` **before** `AddMediarq`, otherwise the scan sees no `IValidator<>` and the
> validation behavior is never wired — validation then silently does nothing. Not an issue with
> `AddMediarqCore()`. See
> [Troubleshooting](https://github.com/rouffou/mediarq/blob/main/docs/guides/troubleshooting.md).

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
