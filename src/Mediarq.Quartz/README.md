# Mediarq.Quartz

Enqueue or schedule a Mediarq command as a **Quartz.NET job** — it runs through the real Mediarq dispatch
pipeline (behaviors, validation, ...) when the trigger fires.

```bash
dotnet add package Mediarq.Quartz
```

## Usage

```csharp
builder.Services.AddMediarqQuartz();
builder.Services.AddQuartz(); // DI job resolution is the default, no extra configuration needed
builder.Services.AddQuartzHostedService();
```

```csharp
public record SendWelcomeEmail(Guid UserId) : ICommand;
```

```csharp
// Run as soon as the scheduler picks up the trigger.
await scheduler.EnqueueAsync(new SendWelcomeEmail(userId));

// Run after a delay.
await scheduler.ScheduleAsync(new SendWelcomeEmail(userId), TimeSpan.FromMinutes(10));

// Run at a specific point in time.
await scheduler.ScheduleAsync(new SendWelcomeEmail(userId), DateTimeOffset.UtcNow.AddDays(1));
```

Only `ICommand` (the no-result form) is supported — a job scheduled for later has no caller left to
observe a return value.

## How it works

The command is JSON-serialized (via `System.Text.Json`) into the job's `JobDataMap` alongside its
`Type.AssemblyQualifiedName`, and reconstructed by the internal job type when the trigger fires. Requires
Quartz's dependency-injection integration (`services.AddQuartz()`, from
`Quartz.Extensions.DependencyInjection`) so the job — and everything the command's handler depends on —
resolves scoped services correctly. For a job to survive an app restart, configure a persistent Quartz job
store (e.g. `UsePersistentStore(...)`) — the in-memory default (like Quartz's own) does not.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
