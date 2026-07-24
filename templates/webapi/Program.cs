using FluentValidation;
using Mediarq.Extensions;
using Mediarq.FluentValidation;
using Mediarq.WebApiTemplate1.Features.Todos;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------------------------------------------------
// Infrastructure the extensions build on. Swap InMemoryTodoStore for a real persistence layer
// (e.g. Mediarq.EntityFrameworkCore) when you outgrow it.
// --------------------------------------------------------------------------------------------------
builder.Services.AddSingleton<ITodoStore, InMemoryTodoStore>();
builder.Services.AddHttpContextAccessor(); // required by HttpUserContext (isHttp: true)

// FluentValidation validators discovered for the FluentValidation adapter.
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Validation adapter BEFORE AddMediarq so the scan wires the ValidationBehavior (open-generic IValidator<>).
builder.Services.AddMediarqFluentValidation();

// Mediarq core. Scans this assembly for handlers, behaviors and validators.
// (For trimming / Native AOT, swap for: AddMediarqCore(isHttp: true).AddMediarqHandlers().)
builder.Services.AddMediarq(isHttp: true, typeof(Program).Assembly)
    .AddMediarqRequestLogging();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapTodoEndpoints();
app.MapGet("/", () => Results.Redirect("/openapi/v1.json"));

app.Run();

// Exposed so the test host / WebApplicationFactory can reference the entry point.
public partial class Program;
