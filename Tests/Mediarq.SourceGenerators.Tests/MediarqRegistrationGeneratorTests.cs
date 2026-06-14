using System;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Mediarq.SourceGenerators.Tests;

public class MediarqRegistrationGeneratorTests
{
    private static (string GeneratedSource, ImmutableArray<Diagnostic> Diagnostics) Run(string source)
    {
        // Ensure the Mediarq assembly (which declares the marker interfaces) is loaded and referenced.
        var mediarqLocation = typeof(Mediarq.Core.Common.Results.Result).Assembly.Location;

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => a.Location)
            .Append(mediarqLocation)
            .Distinct()
            .Select(p => MetadataReference.CreateFromFile(p));

        var compilation = CSharpCompilation.Create(
            "GenTest",
            [CSharpSyntaxTree.ParseText(source)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new MediarqRegistrationGenerator().AsSourceGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

        var result = driver.GetRunResult();
        var generated = result.GeneratedTrees.Length > 0 ? result.GeneratedTrees[0].ToString() : string.Empty;
        return (generated, result.Diagnostics);
    }

    [Fact]
    public void Generates_Registration_For_Command_Handler()
    {
        const string source = """
            using Mediarq.Core.Common.Requests.Command;
            using Mediarq.Core.Common.Results;
            using System.Threading;
            using System.Threading.Tasks;

            namespace Demo;

            public record Ping(string Text) : ICommand<Result<string>>;

            public sealed class PingHandler : ICommandHandler<Ping, Result<string>>
            {
                public Task<Result<string>> Handle(Ping request, CancellationToken cancellationToken = default)
                    => Task.FromResult(Result.Success(request.Text));
            }
            """;

        var (generated, diagnostics) = Run(source);

        diagnostics.Should().BeEmpty();
        generated.Should().Contain("AddMediarqHandlers");
        generated.Should().Contain("global::Demo.PingHandler");
        generated.Should().Contain("IRequestHandler<global::Demo.Ping, global::Mediarq.Core.Common.Results.Result<string>>");

        // The generator also pre-populates the reflection-free dispatch registry...
        generated.Should().Contain("new global::Mediarq.Core.Mediators.MediarqWrapperRegistry()");
        generated.Should().Contain("registry.Add<global::Demo.Ping, global::Mediarq.Core.Common.Results.Result<string>>();");
        generated.Should().Contain("services.AddSingleton(registry);");
        // ...and the AOT-safe validation-failure factory for the Result<T> response.
        generated.Should().Contain("ValidationFailureRegistry.Register<global::Mediarq.Core.Common.Results.Result<string>>");
    }

    [Fact]
    public void Generates_Notification_Wrapper_Registration()
    {
        const string source = """
            using Mediarq.Core.Common.Requests.Notifications;
            using System.Threading;
            using System.Threading.Tasks;

            namespace Demo;

            public record OrderPlaced(int Id) : INotification;

            public sealed class AuditHandler : INotificationHandler<OrderPlaced>
            {
                public Task Handle(OrderPlaced notification, CancellationToken cancellationToken = default) => Task.CompletedTask;
            }

            public sealed class EmailHandler : INotificationHandler<OrderPlaced>
            {
                public Task Handle(OrderPlaced notification, CancellationToken cancellationToken = default) => Task.CompletedTask;
            }
            """;

        var (generated, diagnostics) = Run(source);

        diagnostics.Should().BeEmpty();
        // Both handlers are registered, but the notification wrapper is added exactly once.
        generated.Should().Contain("global::Demo.AuditHandler");
        generated.Should().Contain("global::Demo.EmailHandler");
        generated.Should().Contain("registry.AddNotification<global::Demo.OrderPlaced>();");
        System.Text.RegularExpressions.Regex.Matches(generated, "AddNotification<global::Demo.OrderPlaced>").Count.Should().Be(1);
    }

    [Fact]
    public void Reports_MQ001_For_Multiple_Handlers_Of_Same_Request()
    {
        const string source = """
            using Mediarq.Core.Common.Requests.Command;
            using Mediarq.Core.Common.Results;
            using System.Threading;
            using System.Threading.Tasks;

            namespace Demo;

            public record Ping(string Text) : ICommand<Result<string>>;

            public sealed class PingHandler1 : ICommandHandler<Ping, Result<string>>
            {
                public Task<Result<string>> Handle(Ping request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Text));
            }

            public sealed class PingHandler2 : ICommandHandler<Ping, Result<string>>
            {
                public Task<Result<string>> Handle(Ping request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Text));
            }
            """;

        var (_, diagnostics) = Run(source);

        diagnostics.Should().Contain(d => d.Id == "MQ001");
    }

    [Fact]
    public void Generates_Stream_Wrapper_Registration()
    {
        const string source = """
            using Mediarq.Core.Common.Requests.Streaming;
            using System.Collections.Generic;
            using System.Threading;
            using System.Threading.Tasks;

            namespace Demo;

            public record Ticks(int N) : IStreamRequest<int>;

            public sealed class TicksHandler : IStreamRequestHandler<Ticks, int>
            {
                public async IAsyncEnumerable<int> Handle(Ticks request, CancellationToken cancellationToken = default)
                {
                    for (var i = 0; i < request.N; i++) { yield return i; }
                    await Task.CompletedTask;
                }
            }
            """;

        var (generated, diagnostics) = Run(source);

        diagnostics.Should().BeEmpty();
        generated.Should().Contain("global::Demo.TicksHandler");
        generated.Should().Contain("registry.AddStream<global::Demo.Ticks, int>();");
    }

    [Fact]
    public void Generates_Empty_Method_When_No_Handlers()
    {
        const string source = """
            namespace Demo;

            public class Plain { }
            """;

        var (generated, diagnostics) = Run(source);

        diagnostics.Should().BeEmpty();
        generated.Should().Contain("AddMediarqHandlers");
        generated.Should().NotContain("AddScoped");
    }
}
