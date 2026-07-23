using Mediarq.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Mediarq.Analyzers.Tests;

public class MediatRMigrationAnalyzerTests
{
    // Minimal MediatR surface so the analyzer can bind the MediatR types without the commercial package.
    private const string MediatRStubs = @"
namespace MediatR
{
    public interface IRequest { }
    public interface IRequest<out TResponse> { }
    public interface IRequestHandler<in TRequest, TResponse> { }
    public interface INotification { }
    public interface INotificationHandler<in TNotification> { }
    public interface IPipelineBehavior<in TRequest, TResponse> { }
    public interface ISender { }
    public interface IPublisher { }
    public interface IMediator { }
    public interface IStreamRequest<out TResponse> { }
    public interface IStreamRequestHandler<in TRequest, out TResponse> { }
}
";

    private static async Task VerifyAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<MediatRMigrationAnalyzer, DefaultVerifier>
        {
            TestCode = source + MediatRStubs,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            CompilerDiagnostics = CompilerDiagnostics.None,
        };
        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync();
    }

    private static DiagnosticResult Diagnostic(int markupKey)
        => new DiagnosticResult(MediatRMigrationAnalyzer.DiagnosticId, DiagnosticSeverity.Info).WithLocation(markupKey);

    [Fact]
    public Task Flags_IRequest_Void()
        => VerifyAsync("using MediatR; public class Ping : {|MQ100:IRequest|} { }");

    [Fact]
    public Task Flags_Generic_IRequest()
        => VerifyAsync("using MediatR; public class Ping : {|MQ100:IRequest<string>|} { }");

    [Fact]
    public Task Flags_IRequestHandler()
        => VerifyAsync("using MediatR; public class H : {|MQ100:IRequestHandler<object, string>|} { }");

    [Fact]
    public Task Flags_INotification_And_Handler()
        => VerifyAsync(
            "using MediatR; public class N : {|MQ100:INotification|} { } public class NH : {|MQ100:INotificationHandler<N>|} { }");

    [Fact]
    public Task Flags_IPipelineBehavior()
        => VerifyAsync("using MediatR; public class B : {|MQ100:IPipelineBehavior<object, string>|} { }");

    [Fact]
    public Task Flags_Streaming_Interfaces()
        => VerifyAsync(
            "using MediatR; public class S : {|MQ100:IStreamRequest<string>|} { } public class SH : {|MQ100:IStreamRequestHandler<S, string>|} { }");

    [Fact]
    public Task Flags_Injected_Mediator_Entry_Points()
        => VerifyAsync("using MediatR; public class C { private MediatR.{|MQ100:ISender|} _s; private MediatR.{|MQ100:IMediator|} _m; }");

    [Fact]
    public Task Does_Not_Flag_A_Same_Named_Type_In_Another_Namespace()
        => VerifyAsync(@"
namespace Other { public interface IRequest<T> { } }
public class Ping : Other.IRequest<string> { }");

    [Fact]
    public Task Does_Not_Flag_Plain_Code()
        => VerifyAsync("public class Plain { }");

    [Fact]
    public async Task Reports_The_Ambiguous_Mapping_For_Generic_IRequest()
    {
        await VerifyAsync(
            "using MediatR; public class Ping : {|#0:IRequest<string>|} { }",
            Diagnostic(0).WithArguments("IRequest", "ICommand or IQuery", string.Empty));
    }

    [Fact]
    public async Task Reports_The_One_To_One_Mapping_For_INotification()
    {
        await VerifyAsync(
            "using MediatR; public class N : {|#0:INotification|} { }",
            Diagnostic(0).WithArguments("INotification", "INotification", string.Empty));
    }
}
