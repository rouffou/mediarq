# Mediarq.DataAnnotations

Validate requests with `System.ComponentModel.DataAnnotations` attributes (`[Required]`, `[Range]`,
`[StringLength]`, …) inside the Mediarq pipeline — the attribute-based alternative to FluentValidation.

```bash
dotnet add package Mediarq.DataAnnotations
```

## Usage

```csharp
builder.Services.AddMediarqDataAnnotations();
```

```csharp
public record AddNote : ICommand<Result>
{
    [Required] public Guid OrderId { get; init; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(200, MinimumLength = 1)]
    public string Note { get; init; } = string.Empty;
}
```

Every request is validated against its attributes; requests without any simply pass. An invalid request
short-circuits with a failed `Result` carrying the property errors.

> ⚠️ With the scan core (`AddMediarq(...)`), register this **before** `AddMediarq` so the validation
> behavior is wired. Not an issue with `AddMediarqCore()`. You can combine it with FluentValidation and
> built-in validators — every matching validator runs.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
