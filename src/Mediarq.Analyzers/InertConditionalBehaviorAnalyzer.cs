using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mediarq.Analyzers;

/// <summary>
/// Reports an <c>IConditionalPipelineBehavior.IsActive</c> implementation whose getter is syntactically
/// always the literal <c>false</c> — the behavior is registered but can never actually run for any
/// request type.
/// </summary>
/// <remarks>
/// Like <see cref="PipelineBehaviorNextAnalyzer"/>, this is a syntactic check, not a full flow-analysis
/// proof: it only fires when the getter is literally <c>false</c> (as an expression body, a getter
/// expression body, or a single <c>return false;</c> statement) — a real placeholder/forgotten-logic
/// pattern, not something that could be a legitimate permanent no-op through more complex logic.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InertConditionalBehaviorAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic id reported when a conditional pipeline behavior is never active.</summary>
    public const string DiagnosticId = "MQ204";

    private const string ConditionalBehaviorInterfaceName = "IConditionalPipelineBehavior";
    private const string PipelineNamespace = "Mediarq.Core.Common.Pipeline";
    private const string IsActivePropertyName = "IsActive";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Pipeline behavior is registered but never active",
        messageFormat: "'{0}.IsActive' always returns false, so this behavior never participates in the pipeline for any request",
        category: "Mediarq.Pipeline",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "An IConditionalPipelineBehavior.IsActive implementation that unconditionally returns false means the behavior is registered but can never run — almost always a leftover placeholder or forgotten logic, not an intentional permanent no-op.",
        helpLinkUri: "https://github.com/rouffou/mediarq/blob/main/docs/guides/troubleshooting.md");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        var propertySyntax = (PropertyDeclarationSyntax)context.Node;
        if (propertySyntax.Identifier.ValueText != IsActivePropertyName)
        {
            return;
        }

        var semanticModel = context.SemanticModel;
        if (semanticModel.GetDeclaredSymbol(propertySyntax, context.CancellationToken) is not IPropertySymbol property)
        {
            return;
        }

        var containingType = property.ContainingType;
        var conditionalInterface = containingType?.AllInterfaces.FirstOrDefault(IsConditionalBehaviorInterface);
        if (containingType is null || conditionalInterface is null)
        {
            return;
        }

        var interfaceProperty = conditionalInterface.GetMembers(IsActivePropertyName).OfType<IPropertySymbol>().FirstOrDefault();
        if (interfaceProperty is null)
        {
            return;
        }

        // Confirm this specific property is the interface's implementation (not an unrelated member).
        if (!SymbolEqualityComparer.Default.Equals(containingType.FindImplementationForInterfaceMember(interfaceProperty), property))
        {
            return;
        }

        if (IsAlwaysFalse(propertySyntax))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, propertySyntax.Identifier.GetLocation(), containingType.Name));
        }
    }

    private static bool IsAlwaysFalse(PropertyDeclarationSyntax propertySyntax)
    {
        if (propertySyntax.ExpressionBody is { } expressionBody)
        {
            return IsFalseLiteral(expressionBody.Expression);
        }

        var getter = propertySyntax.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.GetAccessorDeclaration));
        if (getter is null)
        {
            return false; // auto-property, init-only, or no getter -- nothing to check.
        }

        if (getter.ExpressionBody is { } getterExpressionBody)
        {
            return IsFalseLiteral(getterExpressionBody.Expression);
        }

        if (getter.Body is { Statements.Count: 1 } block &&
            block.Statements[0] is ReturnStatementSyntax { Expression: { } returnExpression })
        {
            return IsFalseLiteral(returnExpression);
        }

        return false;
    }

    private static bool IsFalseLiteral(ExpressionSyntax expression) => expression.IsKind(SyntaxKind.FalseLiteralExpression);

    private static bool IsConditionalBehaviorInterface(INamedTypeSymbol iface)
        => iface.Arity == 0 &&
           iface.Name == ConditionalBehaviorInterfaceName &&
           iface.ContainingNamespace?.ToDisplayString() == PipelineNamespace;
}
