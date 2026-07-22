using Mediarq.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Mediarq.Analyzers.Tests;

public class CaptiveDependencyAnalyzerTests
{
    // Minimal stubs for the types the analyzer looks for by name/namespace: the real
    // RegisterHandlerAttribute (over the real ServiceLifetime enum) and a fake DbContext.
    private const string Stubs = @"
namespace Mediarq.Core.Common.Registration
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class RegisterHandlerAttribute : System.Attribute
    {
        public RegisterHandlerAttribute(Microsoft.Extensions.DependencyInjection.ServiceLifetime lifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped) { }
    }
}

namespace Microsoft.EntityFrameworkCore
{
    public class DbContext { }
}
";

    private static async Task VerifyAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<CaptiveDependencyAnalyzer, DefaultVerifier>
        {
            TestCode = source + Stubs,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            CompilerDiagnostics = CompilerDiagnostics.None,
        };
        test.TestState.AdditionalReferences.Add(typeof(Microsoft.Extensions.DependencyInjection.ServiceLifetime).Assembly);
        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync();
    }

    private static DiagnosticResult Diagnostic(int markupKey)
        => new DiagnosticResult(CaptiveDependencyAnalyzer.DiagnosticId, DiagnosticSeverity.Warning).WithLocation(markupKey);

    [Fact]
    public async Task Flags_Singleton_Depending_On_DbContext()
    {
        const string source = @"
using Mediarq.Core.Common.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

[RegisterHandler(ServiceLifetime.Singleton)]
public class MyBehavior
{
    public MyBehavior(DbContext {|#0:db|}) { }
}
";
        await VerifyAsync(source, Diagnostic(0).WithArguments("MyBehavior", "DbContext", "which is a DbContext (scoped by convention)"));
    }

    [Fact]
    public async Task Flags_Singleton_Depending_On_Explicit_Scoped()
    {
        const string source = @"
using Mediarq.Core.Common.Registration;
using Microsoft.Extensions.DependencyInjection;

[RegisterHandler(ServiceLifetime.Scoped)]
public class Dependency { }

[RegisterHandler(ServiceLifetime.Singleton)]
public class MyBehavior
{
    public MyBehavior(Dependency {|#0:dependency|}) { }
}
";
        await VerifyAsync(source, Diagnostic(0).WithArguments("MyBehavior", "Dependency", "registered as Scoped"));
    }

    [Fact]
    public async Task Flags_Singleton_Depending_On_Explicit_Transient()
    {
        const string source = @"
using Mediarq.Core.Common.Registration;
using Microsoft.Extensions.DependencyInjection;

[RegisterHandler(ServiceLifetime.Transient)]
public class Dependency { }

[RegisterHandler(ServiceLifetime.Singleton)]
public class MyBehavior
{
    public MyBehavior(Dependency {|#0:dependency|}) { }
}
";
        await VerifyAsync(source, Diagnostic(0).WithArguments("MyBehavior", "Dependency", "registered as Transient"));
    }

    [Fact]
    public Task Does_Not_Flag_Singleton_Depending_On_Another_Singleton()
        => VerifyAsync(@"
using Mediarq.Core.Common.Registration;
using Microsoft.Extensions.DependencyInjection;

[RegisterHandler(ServiceLifetime.Singleton)]
public class Dependency { }

[RegisterHandler(ServiceLifetime.Singleton)]
public class MyBehavior
{
    public MyBehavior(Dependency dependency) { }
}
");

    [Fact]
    public Task Does_Not_Flag_Singleton_Depending_On_An_Undecorated_Type()
        => VerifyAsync(@"
using Mediarq.Core.Common.Registration;
using Microsoft.Extensions.DependencyInjection;

// No [RegisterHandler]: default Scoped, but not an explicit signal we compare against.
public class Dependency { }

[RegisterHandler(ServiceLifetime.Singleton)]
public class MyBehavior
{
    public MyBehavior(Dependency dependency) { }
}
");

    [Fact]
    public Task Does_Not_Flag_A_Scoped_Type_Depending_On_DbContext()
        => VerifyAsync(@"
using Mediarq.Core.Common.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

[RegisterHandler(ServiceLifetime.Scoped)]
public class MyBehavior
{
    public MyBehavior(DbContext db) { }
}
");

    [Fact]
    public Task Does_Not_Flag_Plain_Code()
        => VerifyAsync("public class Plain { public Plain(object dependency) { } }");
}
