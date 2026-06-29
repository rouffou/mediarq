using Mediarq.Analyzers;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Mediarq.Analyzers.Tests;

public class MediatRMigrationCodeFixTests
{
    // Both the source MediatR surface and the Mediarq targets, so the migrated code binds to Mediarq and
    // the analyzer does not flag it again (otherwise the harness would apply a second fix iteration).
    private const string MediatRStubs = @"
namespace MediatR
{
    public interface IRequest { }
    public interface IRequest<out TResponse> { }
    public interface INotification { }
    public interface ISender { }
}
namespace Mediarq.Core.Common.Requests.Command { public interface ICommand { } public interface ICommand<out TResponse> { } }
namespace Mediarq.Core.Common.Requests.Query { public interface IQuery<out TResponse> { } }
namespace Mediarq.Core.Common.Requests.Notifications { public interface INotification { } }
namespace Mediarq.Core.Mediators { public interface ISender { } }
";

    private static async Task VerifyFixAsync(string source, string fixedSource, string? equivalenceKey = null)
    {
        var test = new CSharpCodeFixTest<MediatRMigrationAnalyzer, MediatRMigrationCodeFixProvider, DefaultVerifier>
        {
            TestCode = source + MediatRStubs,
            FixedCode = fixedSource + MediatRStubs,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            CompilerDiagnostics = CompilerDiagnostics.None,
            CodeActionEquivalenceKey = equivalenceKey,
        };
        await test.RunAsync();
    }

    [Fact]
    public Task Rewrites_INotification_And_Adds_The_Namespace()
        => VerifyFixAsync(
            "using MediatR;\r\n\r\npublic class N : {|MQ100:INotification|} { }\r\n",
            "using MediatR;\r\nusing Mediarq.Core.Common.Requests.Notifications;\r\n\r\npublic class N : INotification { }\r\n");

    [Fact]
    public Task Rewrites_Void_IRequest_To_ICommand()
        => VerifyFixAsync(
            "using MediatR;\r\n\r\npublic class Ping : {|MQ100:IRequest|} { }\r\n",
            "using MediatR;\r\nusing Mediarq.Core.Common.Requests.Command;\r\n\r\npublic class Ping : ICommand { }\r\n");

    [Fact]
    public Task Rewrites_Generic_IRequest_To_ICommand_When_That_Fix_Is_Chosen()
        => VerifyFixAsync(
            "using MediatR;\r\n\r\npublic class Ping : {|MQ100:IRequest<string>|} { }\r\n",
            "using MediatR;\r\nusing Mediarq.Core.Common.Requests.Command;\r\n\r\npublic class Ping : ICommand<string> { }\r\n",
            equivalenceKey: "ICommand");

    [Fact]
    public Task Rewrites_Generic_IRequest_To_IQuery_When_That_Fix_Is_Chosen()
        => VerifyFixAsync(
            "using MediatR;\r\n\r\npublic class Ping : {|MQ100:IRequest<string>|} { }\r\n",
            "using MediatR;\r\nusing Mediarq.Core.Common.Requests.Query;\r\n\r\npublic class Ping : IQuery<string> { }\r\n",
            equivalenceKey: "IQuery");

    [Fact]
    public Task Rewrites_Injected_ISender()
        => VerifyFixAsync(
            "using MediatR;\r\n\r\npublic class C { private {|MQ100:ISender|} _s; }\r\n",
            "using MediatR;\r\nusing Mediarq.Core.Mediators;\r\n\r\npublic class C { private ISender _s; }\r\n");

    [Fact]
    public Task Rewrites_A_Fully_Qualified_Usage()
        => VerifyFixAsync(
            "public class N : MediatR.{|MQ100:INotification|} { }\r\n",
            "using Mediarq.Core.Common.Requests.Notifications;\r\npublic class N : INotification { }\r\n");
}
