using Mediarq.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Mediarq.Analyzers.Tests;

public class InertConditionalBehaviorAnalyzerTests
{
    // Minimal stubs for the types the analyzer looks for by name/namespace/arity.
    private const string Stubs = @"
namespace Mediarq.Core.Common.Pipeline
{
    public interface IPipelineBehavior<TRequest, TResponse>
    {
        System.Threading.Tasks.Task<TResponse> Handle(object context, System.Func<System.Threading.Tasks.Task<TResponse>> handle, System.Threading.CancellationToken cancellationToken = default);
    }

    public interface IConditionalPipelineBehavior
    {
        bool IsActive { get; }
    }
}
";

    private static async Task VerifyAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<InertConditionalBehaviorAnalyzer, DefaultVerifier>
        {
            TestCode = source + Stubs,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            CompilerDiagnostics = CompilerDiagnostics.None,
        };
        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync();
    }

    private static DiagnosticResult Diagnostic(int markupKey)
        => new DiagnosticResult(InertConditionalBehaviorAnalyzer.DiagnosticId, DiagnosticSeverity.Warning).WithLocation(markupKey);

    [Fact]
    public async Task Flags_An_Expression_Bodied_Property_That_Always_Returns_False()
    {
        const string source = @"
using Mediarq.Core.Common.Pipeline;

public class NeverActiveBehavior : IPipelineBehavior<object, string>, IConditionalPipelineBehavior
{
    public bool {|#0:IsActive|} => false;

    public System.Threading.Tasks.Task<string> Handle(object context, System.Func<System.Threading.Tasks.Task<string>> handle, System.Threading.CancellationToken cancellationToken = default)
        => handle();
}
";
        await VerifyAsync(source, Diagnostic(0).WithArguments("NeverActiveBehavior"));
    }

    [Fact]
    public async Task Flags_A_Getter_With_An_Expression_Body_That_Always_Returns_False()
    {
        const string source = @"
using Mediarq.Core.Common.Pipeline;

public class NeverActiveBehavior : IPipelineBehavior<object, string>, IConditionalPipelineBehavior
{
    public bool {|#0:IsActive|} { get => false; }

    public System.Threading.Tasks.Task<string> Handle(object context, System.Func<System.Threading.Tasks.Task<string>> handle, System.Threading.CancellationToken cancellationToken = default)
        => handle();
}
";
        await VerifyAsync(source, Diagnostic(0).WithArguments("NeverActiveBehavior"));
    }

    [Fact]
    public async Task Flags_A_Block_Getter_That_Always_Returns_False()
    {
        const string source = @"
using Mediarq.Core.Common.Pipeline;

public class NeverActiveBehavior : IPipelineBehavior<object, string>, IConditionalPipelineBehavior
{
    public bool {|#0:IsActive|}
    {
        get
        {
            return false;
        }
    }

    public System.Threading.Tasks.Task<string> Handle(object context, System.Func<System.Threading.Tasks.Task<string>> handle, System.Threading.CancellationToken cancellationToken = default)
        => handle();
}
";
        await VerifyAsync(source, Diagnostic(0).WithArguments("NeverActiveBehavior"));
    }

    [Fact]
    public async Task Flags_An_Explicit_Interface_Implementation_That_Always_Returns_False()
    {
        const string source = @"
using Mediarq.Core.Common.Pipeline;

public class NeverActiveBehavior : IPipelineBehavior<object, string>, IConditionalPipelineBehavior
{
    bool IConditionalPipelineBehavior.{|#0:IsActive|} => false;

    public System.Threading.Tasks.Task<string> Handle(object context, System.Func<System.Threading.Tasks.Task<string>> handle, System.Threading.CancellationToken cancellationToken = default)
        => handle();
}
";
        await VerifyAsync(source, Diagnostic(0).WithArguments("NeverActiveBehavior"));
    }

    [Fact]
    public Task Does_Not_Flag_A_Property_That_Always_Returns_True()
        => VerifyAsync(@"
using Mediarq.Core.Common.Pipeline;

public class AlwaysActiveBehavior : IPipelineBehavior<object, string>, IConditionalPipelineBehavior
{
    public bool IsActive => true;

    public System.Threading.Tasks.Task<string> Handle(object context, System.Func<System.Threading.Tasks.Task<string>> handle, System.Threading.CancellationToken cancellationToken = default)
        => handle();
}
");

    [Fact]
    public Task Does_Not_Flag_A_Property_With_Conditional_Logic()
        => VerifyAsync(@"
using Mediarq.Core.Common.Pipeline;

public class ConditionalBehavior : IPipelineBehavior<object, string>, IConditionalPipelineBehavior
{
    private static readonly object[] Validators = System.Array.Empty<object>();

    public bool IsActive => Validators.Length > 0;

    public System.Threading.Tasks.Task<string> Handle(object context, System.Func<System.Threading.Tasks.Task<string>> handle, System.Threading.CancellationToken cancellationToken = default)
        => handle();
}
");

    [Fact]
    public Task Does_Not_Flag_An_Unrelated_IsActive_Property()
        => VerifyAsync(@"
public class NotABehavior
{
    public bool IsActive => false;
}
");

    [Fact]
    public Task Does_Not_Flag_A_Behavior_That_Is_Not_Conditional()
        => VerifyAsync(@"
using Mediarq.Core.Common.Pipeline;

public class UnconditionalBehavior : IPipelineBehavior<object, string>
{
    public bool IsActive => false; // not IConditionalPipelineBehavior -- unrelated member, always runs

    public System.Threading.Tasks.Task<string> Handle(object context, System.Func<System.Threading.Tasks.Task<string>> handle, System.Threading.CancellationToken cancellationToken = default)
        => handle();
}
");
}
