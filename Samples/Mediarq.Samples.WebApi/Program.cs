using FluentValidation;
using Mediarq.Caching;
using Mediarq.DataAnnotations;
using Mediarq.Diagnostics;
using Mediarq.EntityFrameworkCore;
using Mediarq.Extensions;
using Mediarq.FluentValidation;
using Mediarq.Idempotency;
using Mediarq.MassTransit;
using Mediarq.OpenTelemetry;
using Mediarq.Outbox;
using Mediarq.Polly;
using Mediarq.Samples.WebApi.Domain;
using Mediarq.Samples.WebApi.Endpoints;
using Mediarq.Samples.WebApi.Features.Orders;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Polly;
using Polly.Retry;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------------------------------------------------
// Infrastructure the extensions build on.
// --------------------------------------------------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("orders"));
builder.Services.AddDistributedMemoryCache();       // backs Mediarq.Idempotency (use Redis in production)
builder.Services.AddHttpContextAccessor();          // required by HttpUserContext (isHttp: true)
builder.Services.AddSingleton<IPricingService, FlakyPricingService>();

// FluentValidation validators discovered for the FluentValidation adapter.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// --------------------------------------------------------------------------------------------------
// Validation adapters BEFORE AddMediarq so the scan wires the ValidationBehavior (open-generic IValidator<>).
// --------------------------------------------------------------------------------------------------
builder.Services.AddMediarqFluentValidation();      // CreateOrderCommand
builder.Services.AddMediarqDataAnnotations();        // AddOrderNoteCommand

// --------------------------------------------------------------------------------------------------
// Mediarq core. Scans this assembly for handlers, behaviors, validators, processors, stream handlers.
// (For trimming / Native AOT, swap for: AddMediarqCore(isHttp: true).AddMediarqHandlers() — see AotSample.)
// --------------------------------------------------------------------------------------------------
builder.Services.AddMediarq(isHttp: true, typeof(Program).Assembly)
    .AddMediarqRequestLogging();

// --------------------------------------------------------------------------------------------------
// Extensions (after AddMediarq).
// --------------------------------------------------------------------------------------------------
builder.Services.AddMediarqEntityFrameworkCore<AppDbContext>();                         // DbContext as unit of work
builder.Services.AddMediarqOutbox<AppDbContext>(o => o.PollingInterval = TimeSpan.FromSeconds(2)); // transactional outbox
builder.Services.AddMediarqCaching();                                                    // ICacheableRequest -> IMemoryCache
builder.Services.AddMediarqIdempotency();                                                // IIdempotentRequest -> IDistributedCache
builder.Services.AddMediarqResilience();                                                 // IResilientRequest -> Polly
builder.Services.AddMediarqDiagnostics();                                                // Activity + metrics (decorates the publisher)

// Polly pipeline referenced by QuotePriceQuery.ResiliencePipelineName.
builder.Services.AddResiliencePipeline("orders-pricing", pipeline => pipeline
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromMilliseconds(50),
        BackoffType = DelayBackoffType.Constant,
    }));

// OpenTelemetry: export the Mediarq activities/metrics to the console.
builder.Services.AddOpenTelemetry()
    .WithTracing(t => t.AddMediarqInstrumentation().AddConsoleExporter())
    .WithMetrics(m => m.AddMediarqInstrumentation().AddConsoleExporter());

// MassTransit (in-memory bus) + the Mediarq forwarder for OrderPlacedEvent.
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();
    x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
});
builder.Services.AddMediarqMassTransitForwarding<OrderPlacedEvent>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options
        .WithTitle("Mediarq — Orders sample")
        .WithTheme(ScalarTheme.Saturn));
}

app.MapOrderEndpoints();
app.MapGet("/", () => Results.Redirect("/scalar/v1"));

app.Run();

// Exposed so the test host / WebApplicationFactory can reference the entry point.
public partial class Program;
