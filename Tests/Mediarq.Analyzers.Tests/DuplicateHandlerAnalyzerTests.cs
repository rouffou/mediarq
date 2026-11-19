using Mediarq.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Mediarq.Analyzers.Tests;

public class DuplicateHandlerAnalyzerTests
{
    // Minimal stubs mirroring the namespace and shape of the real handler interfaces.
    private const string Stubs = @"
namespace Mediarq.Core.Common.Requests.Abstraction
{
    public interface IRequestHandler<in TRequest, TResponse> { }
    public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit> { }
    public struct Unit { }
}

namespace Mediarq.Core.Common.Requests.Command
{
    using Mediarq.Core.Common.Requests.Abstraction;
    public interface ICommandHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse> { }
    public interface ICommandHandler<in TRequest> : IRequestHandler<TRequest> { }
}

namespace Mediarq.Core.Common.Requests.Query
{
    using Mediarq.Core.Common.Requests.Abstraction;
    public interface IQueryHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse> { }
}
";

    private static async Task VerifyAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<DuplicateHandlerAnalyzer, DefaultVerifier>
        {
            TestCode = source + Stubs,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            CompilerDiagnostics = CompilerDiagnostics.None,
        };
        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync();
    }

    private static DiagnosticResult Diagnostic(int markupKey)
        => new DiagnosticResult(DuplicateHandlerAnalyzer.DiagnosticId, DiagnosticSeverity.Warning).WithLocation(markupKey);

    // The first handler in fully-qualified-name order is canonical; every later one is flagged.
    [Fact]
    public async Task Flags_A_Second_Handler_For_The_Same_Request()
    {
        const string source = @"
using Mediarq.Core.Common.Requests.Command;

public class CreateOrder { }
public class AHandler : ICommandHandler<CreateOrder, int> { }
public class {|#0:BHandler|} : ICommandHandler<CreateOrder, int> { }
";
        await VerifyAsync(source, Diagnostic(0));
    }

    [Fact]
    public async Task Flags_Duplicates_Across_Handler_Interface_Kinds()
    {
        const string source = @"
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Query;

public class GetOrder { }
public class AHandler : IQueryHandler<GetOrder, string> { }
public class {|#0:BHandler|} : IRequestHandler<GetOrder, string> { }
";
        await VerifyAsync(source, Diagnostic(0));
    }

    [Fact]
    public async Task Flags_Duplicates_For_A_No_Result_Command()
    {
        const string source = @"
using Mediarq.Core.Common.Requests.Command;

public class Ping { }
public class AHandler : ICommandHandler<Ping> { }
public class {|#0:BHandler|} : ICommandHandler<Ping> { }
";
        await VerifyAsync(source, Diagnostic(0));
    }

    [Fact]
    public async Task Flags_Every_Handler_After_The_First_When_There_Are_Three()
    {
        const string source = @"
using Mediarq.Core.Common.Requests.Command;

public class CreateOrder { }
public class AHandler : ICommandHandler<CreateOrder, int> { }
public class {|#0:BHandler|} : ICommandHandler<CreateOrder, int> { }
public class {|#1:CHandler|} : ICommandHandler<CreateOrder, int> { }
";
        await VerifyAsync(source, Diagnostic(0), Diagnostic(1));
    }

    [Fact]
    public async Task Does_Not_Flag_A_Single_Handler_Per_Request()
    {
        const string source = @"
using Mediarq.Core.Common.Requests.Command;

public class CreateOrder { }
public class CancelOrder { }
public class CreateHandler : ICommandHandler<CreateOrder, int> { }
public class CancelHandler : ICommandHandler<CancelOrder, int> { }
";
        await VerifyAsync(source);
    }

    [Fact]
    public async Task Does_Not_Flag_One_Class_Handling_Several_Requests()
    {
        const string source = @"
using Mediarq.Core.Common.Requests.Command;

public class CreateOrder { }
public class CancelOrder { }
public class OrderHandler : ICommandHandler<CreateOrder, int>, ICommandHandler<CancelOrder, int> { }
";
        await VerifyAsync(source);
    }

    [Fact]
    public async Task Does_Not_Flag_Abstract_Or_Open_Generic_Handlers()
    {
        const string source = @"
using Mediarq.Core.Common.Requests.Command;

public class CreateOrder { }
public abstract class BaseHandler : ICommandHandler<CreateOrder, int> { }
public class GenericHandler<T> : ICommandHandler<CreateOrder, int> { }
public class RealHandler : ICommandHandler<CreateOrder, int> { }
";
        await VerifyAsync(source);
    }

    [Fact]
    public async Task Does_Not_Flag_Unrelated_Types_Named_Like_The_Interface()
    {
        const string source = @"
namespace Other
{
    public interface IRequestHandler<in TRequest, TResponse> { }
}

public class CreateOrder { }
public class AHandler : Other.IRequestHandler<CreateOrder, int> { }
public class BHandler : Other.IRequestHandler<CreateOrder, int> { }
";
        await VerifyAsync(source);
    }
}
