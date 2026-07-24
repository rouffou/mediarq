using Mediarq.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Mediarq.Analyzers.Tests;

public class PipelineBehaviorNextAnalyzerTests
{
    // Minimal stubs for the types the analyzer looks for by name/namespace/arity.
    private const string Stubs = @"
namespace Mediarq.Core.Common.Pipeline
{
    public interface IPipelineBehavior<TRequest, TResponse>
    {
        System.Threading.Tasks.Task<TResponse> Handle(object context, System.Func<System.Threading.Tasks.Task<TResponse>> handle, System.Threading.CancellationToken cancellationToken = default);
    }
}
";

    private static async Task VerifyAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<PipelineBehaviorNextAnalyzer, DefaultVerifier>
        {
            TestCode = source + Stubs,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            CompilerDiagnostics = CompilerDiagnostics.None,
        };
        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync();
    }

    private static DiagnosticResult Diagnostic(int markupKey)
        => new DiagnosticResult(PipelineBehaviorNextAnalyzer.DiagnosticId, DiagnosticSeverity.Warning).WithLocation(markupKey);

    [Fact]
    public async Task Flags_A_Behavior_That_Never_References_The_Handle_Delegate()
    {
        const string source = @"
using Mediarq.Core.Common.Pipeline;

public class ForgetfulBehavior : IPipelineBehavior<object, string>
{
    public System.Threading.Tasks.Task<string> {|#0:Handle|}(object context, System.Func<System.Threading.Tasks.Task<string>> handle, System.Threading.CancellationToken cancellationToken = default)
    {
        return System.Threading.Tasks.Task.FromResult(""short-circuited"");
    }
}
";
        await VerifyAsync(source, Diagnostic(0).WithArguments("ForgetfulBehavior", "handle"));
    }

    [Fact]
    public async Task Flags_An_Expression_Bodied_Behavior_That_Never_References_The_Handle_Delegate()
    {
        const string source = @"
using Mediarq.Core.Common.Pipeline;

public class ForgetfulBehavior : IPipelineBehavior<object, string>
{
    public System.Threading.Tasks.Task<string> {|#0:Handle|}(object context, System.Func<System.Threading.Tasks.Task<string>> handle, System.Threading.CancellationToken cancellationToken = default)
        => System.Threading.Tasks.Task.FromResult(""short-circuited"");
}
";
        await VerifyAsync(source, Diagnostic(0).WithArguments("ForgetfulBehavior", "handle"));
    }

    [Fact]
    public Task Does_Not_Flag_A_Behavior_That_Calls_The_Handle_Delegate()
        => VerifyAsync(@"
using Mediarq.Core.Common.Pipeline;

public class LoggingBehavior : IPipelineBehavior<object, string>
{
    public async System.Threading.Tasks.Task<string> Handle(object context, System.Func<System.Threading.Tasks.Task<string>> handle, System.Threading.CancellationToken cancellationToken = default)
    {
        return await handle();
    }
}
");

    [Fact]
    public Task Does_Not_Flag_A_Behavior_That_Conditionally_Calls_The_Handle_Delegate()
        => VerifyAsync(@"
using Mediarq.Core.Common.Pipeline;

public class CachingBehavior : IPipelineBehavior<object, string>
{
    private static bool _cached;

    public async System.Threading.Tasks.Task<string> Handle(object context, System.Func<System.Threading.Tasks.Task<string>> handle, System.Threading.CancellationToken cancellationToken = default)
    {
        if (_cached)
        {
            return ""cached"";
        }

        return await handle();
    }
}
");

    [Fact]
    public Task Does_Not_Flag_An_Explicit_Interface_Implementation_That_Calls_The_Handle_Delegate()
        => VerifyAsync(@"
using Mediarq.Core.Common.Pipeline;

public class LoggingBehavior : IPipelineBehavior<object, string>
{
    async System.Threading.Tasks.Task<string> IPipelineBehavior<object, string>.Handle(object context, System.Func<System.Threading.Tasks.Task<string>> handle, System.Threading.CancellationToken cancellationToken)
    {
        return await handle();
    }
}
");

    [Fact]
    public Task Does_Not_Flag_An_Abstract_Handle_Method()
        => VerifyAsync(@"
using Mediarq.Core.Common.Pipeline;

public abstract class BehaviorBase : IPipelineBehavior<object, string>
{
    public abstract System.Threading.Tasks.Task<string> Handle(object context, System.Func<System.Threading.Tasks.Task<string>> handle, System.Threading.CancellationToken cancellationToken = default);
}
");

    [Fact]
    public Task Does_Not_Flag_An_Unrelated_Handle_Method()
        => VerifyAsync(@"
public class NotABehavior
{
    public System.Threading.Tasks.Task<string> Handle(object context, System.Func<System.Threading.Tasks.Task<string>> handle, System.Threading.CancellationToken cancellationToken = default)
    {
        return System.Threading.Tasks.Task.FromResult(""whatever"");
    }
}
");
}
