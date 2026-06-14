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
