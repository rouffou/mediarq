using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mediarq.Analyzers;

/// <summary>
/// Reports an <c>IPipelineBehavior&lt;TRequest, TResponse&gt;.Handle</c> implementation that never
/// references its <c>handle</c> delegate parameter anywhere in its body — the classic "forgot to call
/// next" pipeline bug, which silently short-circuits the rest of the pipeline and the actual request
/// handler (they simply never run).
/// </summary>
/// <remarks>
/// The check only requires the parameter identifier to appear *somewhere* in the body, on any code path
/// (e.g. a caching behavior that returns early on a cache hit but still calls it on a miss is not
/// flagged) — it fires only when the delegate is never referenced at all, which is always a mistake, not
/// a legitimate conditional short-circuit.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PipelineBehaviorNextAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic id reported when a pipeline behavior never calls its <c>handle</c> delegate.</summary>
    public const string DiagnosticId = "MQ201";

    private const string PipelineBehaviorInterfaceName = "IPipelineBehavior";
    private const string PipelineBehaviorNamespace = "Mediarq.Core.Common.Pipeline";
    private const string HandleMethodName = "Handle";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Pipeline behavior never calls its 'handle' delegate",
        messageFormat: "'{0}.Handle' never calls the '{1}' delegate — the rest of the pipeline (and the actual request handler) will never run",
        category: "Mediarq.Pipeline",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "An IPipelineBehavior<TRequest, TResponse>.Handle implementation that never references its 'handle' delegate parameter short-circuits the pipeline on every call — almost always a bug rather than an intentional short-circuit.",
        helpLinkUri: "https://github.com/rouffou/mediarq/blob/main/docs/guides/troubleshooting.md");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var methodSyntax = (MethodDeclarationSyntax)context.Node;
        if (methodSyntax.Identifier.ValueText != HandleMethodName)
        {
            return;
        }

        SyntaxNode? body = methodSyntax.Body ?? (SyntaxNode?)methodSyntax.ExpressionBody;
        if (body is null)
        {
            return; // abstract, partial without a body, or extern — nothing to check.
        }

        var semanticModel = context.SemanticModel;
        if (semanticModel.GetDeclaredSymbol(methodSyntax, context.CancellationToken) is not IMethodSymbol method)
        {
            return;
        }

        var containingType = method.ContainingType;
        var behaviorInterface = containingType?.AllInterfaces.FirstOrDefault(IsPipelineBehaviorInterface);
        if (containingType is null || behaviorInterface is null)
        {
            return;
        }

        var interfaceHandleMethod = behaviorInterface.GetMembers(HandleMethodName).OfType<IMethodSymbol>().FirstOrDefault();
        if (interfaceHandleMethod is null)
        {
            return;
        }

        // Confirm this specific method is the interface's implementation (not an unrelated overload).
        if (!SymbolEqualityComparer.Default.Equals(containingType.FindImplementationForInterfaceMember(interfaceHandleMethod), method))
        {
            return;
        }

        // IPipelineBehavior<,>.Handle(context, handle, cancellationToken) — the delegate is parameter 1.
        if (method.Parameters.Length < 2)
        {
            return;
        }

        var delegateParameter = method.Parameters[1];

        var referencesDelegate = body.DescendantNodes()
            .OfType<IdentifierNameSyntax>()
            .Any(id => SymbolEqualityComparer.Default.Equals(
                semanticModel.GetSymbolInfo(id, context.CancellationToken).Symbol, delegateParameter));

        if (!referencesDelegate)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, methodSyntax.Identifier.GetLocation(), containingType.Name, delegateParameter.Name));
        }
    }

    private static bool IsPipelineBehaviorInterface(INamedTypeSymbol iface)
        => iface.Arity == 2 &&
           iface.Name == PipelineBehaviorInterfaceName &&
           iface.ContainingNamespace?.ToDisplayString() == PipelineBehaviorNamespace;
}
