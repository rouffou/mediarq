using Mediarq.Extensions;
using Microsoft.AspNetCore.Builder;
using Scalar.AspNetCore;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediarq(false);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Samples example for this lib");
        options.WithTheme(ScalarTheme.Saturn); // Dark mode
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
