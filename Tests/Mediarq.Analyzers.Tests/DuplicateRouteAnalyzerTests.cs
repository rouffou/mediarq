using Mediarq.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Mediarq.Analyzers.Tests;

public class DuplicateRouteAnalyzerTests
{
    // Minimal stubs for the attributes the analyzer looks for by name/namespace.
    private const string Stubs = @"
namespace Mediarq.AspNetCore
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct, Inherited = false)]
    public sealed class MediarqGetAttribute : System.Attribute
    {
        public MediarqGetAttribute(string pattern) => Pattern = pattern;
        public string Pattern { get; }
    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct, Inherited = false)]
    public sealed class MediarqPostAttribute : System.Attribute
    {
        public MediarqPostAttribute(string pattern) => Pattern = pattern;
        public string Pattern { get; }
    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct, Inherited = false)]
    public sealed class MediarqPutAttribute : System.Attribute
    {
        public MediarqPutAttribute(string pattern) => Pattern = pattern;
        public string Pattern { get; }
    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct, Inherited = false)]
    public sealed class MediarqPatchAttribute : System.Attribute
    {
        public MediarqPatchAttribute(string pattern) => Pattern = pattern;
        public string Pattern { get; }
    }

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct, Inherited = false)]
    public sealed class MediarqDeleteAttribute : System.Attribute
    {
        public MediarqDeleteAttribute(string pattern) => Pattern = pattern;
        public string Pattern { get; }
    }
}
";

    private static async Task VerifyAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<DuplicateRouteAnalyzer, DefaultVerifier>
        {
            TestCode = source + Stubs,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            CompilerDiagnostics = CompilerDiagnostics.None,
        };
        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync();
    }

    private static DiagnosticResult Diagnostic(int markupKey)
        => new DiagnosticResult(DuplicateRouteAnalyzer.DiagnosticId, DiagnosticSeverity.Warning).WithLocation(markupKey);

    // Diagnostics are reported deterministically ordered by fully-qualified type name; the first name
    // in that order is treated as the "canonical" declaration and every later one is flagged against it.
    [Fact]
    public async Task Flags_Two_Types_Declaring_The_Same_Method_And_Pattern()
    {
        const string source = @"
using Mediarq.AspNetCore;

[{|#0:MediarqGet(""/orders/{id}"")|}]
public class GetOrder { }

[MediarqGet(""/orders/{id}"")]
public class FetchOrder { }
";
        await VerifyAsync(source, Diagnostic(0).WithArguments("GetOrder", "FetchOrder", "GET", "/orders/{id}"));
    }

    [Fact]
    public async Task Flags_Every_Type_After_The_First_When_Three_Types_Collide()
    {
        const string source = @"
using Mediarq.AspNetCore;

[{|#0:MediarqGet(""/orders/{id}"")|}]
public class GetOrder { }

[MediarqGet(""/orders/{id}"")]
public class FetchOrder { }

[{|#1:MediarqGet(""/orders/{id}"")|}]
public class LoadOrder { }
";
        await VerifyAsync(
            source,
            Diagnostic(0).WithArguments("GetOrder", "FetchOrder", "GET", "/orders/{id}"),
            Diagnostic(1).WithArguments("LoadOrder", "FetchOrder", "GET", "/orders/{id}"));
    }

    [Fact]
    public Task Does_Not_Flag_Same_Pattern_With_Different_Methods()
        => VerifyAsync(@"
using Mediarq.AspNetCore;

[MediarqGet(""/orders/{id}"")]
public class GetOrder { }

[MediarqDelete(""/orders/{id}"")]
public class DeleteOrder { }
");

    [Fact]
    public Task Does_Not_Flag_Different_Patterns_With_The_Same_Method()
        => VerifyAsync(@"
using Mediarq.AspNetCore;

[MediarqGet(""/orders/{id}"")]
public class GetOrder { }

[MediarqGet(""/orders/{id}/items"")]
public class GetOrderItems { }
");

    [Fact]
    public Task Does_Not_Flag_A_Single_Type_Declaring_A_Route()
        => VerifyAsync(@"
using Mediarq.AspNetCore;

[MediarqPost(""/orders"")]
public class CreateOrder { }
");

    [Fact]
    public Task Does_Not_Flag_An_Unrelated_Attribute_With_The_Same_Name()
        => VerifyAsync(@"
[MediarqGet(""/orders/{id}"")]
public class GetOrder { }

[MediarqGet(""/orders/{id}"")]
public class FetchOrder { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class MediarqGetAttribute : System.Attribute
{
    public MediarqGetAttribute(string pattern) { }
}
");
}
