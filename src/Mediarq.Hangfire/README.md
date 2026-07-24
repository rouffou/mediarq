# Mediarq.Hangfire

Enqueue or schedule a Mediarq command as a **Hangfire background job** — it runs through the real
Mediarq dispatch pipeline (behaviors, validation, ...) when the job executes.

```bash
dotnet add package Mediarq.Hangfire
```

## Usage

```csharp
builder.Services.AddMediarqHangfire();
builder.Services.AddHangfire(cfg => cfg.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();
```

```csharp
public record SendWelcomeEmail(Guid UserId) : ICommand;
```

```csharp
// Run as soon as a worker is free.
backgroundJobClient.Enqueue(new SendWelcomeEmail(userId));

// Run after a delay.
backgroundJobClient.Schedule(new SendWelcomeEmail(userId), TimeSpan.FromMinutes(10));

// Run at a specific point in time.
backgroundJobClient.Schedule(new SendWelcomeEmail(userId), DateTimeOffset.UtcNow.AddDays(1));
```

Only `ICommand` (the no-result form) is supported — a job scheduled for later has no caller left to
observe a return value, so there is nothing meaningful for a query or a `Result<T>`-returning command to
report back to. If the handler fails, Hangfire's own retry policy applies as normal.

## How it works

`Enqueue`/`Schedule` don't serialize your command against the `ICommand` marker interface — each is a
generic method, so the *concrete* command type is captured at the call site and that's what Hangfire's
job serializer stores. (Passing a variable statically typed as `ICommand` itself would make Hangfire
store the interface as the parameter type and fail to deserialize the concrete command back — always
call these extensions directly on the command instance, not through an `ICommand`-typed variable.)

Requires a running Hangfire server (`AddHangfireServer()`) whose job activator resolves scoped services
(the default `services.AddHangfire(...)` wiring from `Hangfire.NetCore`/`Hangfire.AspNetCore` does this
automatically) so `IMediarqJobDispatcher` — and everything the command's handler depends on — resolves
correctly when the job runs.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
