using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mediarq.Analyzers;

/// <summary>
/// Reports MediatR types that have a Mediarq equivalent (<c>IRequest</c>, <c>IRequestHandler</c>,
/// <c>INotification</c>, <c>IPipelineBehavior</c>, the streaming interfaces and the mediator entry
/// points), so they can be rewritten by <see cref="MediatRMigrationCodeFixProvider"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MediatRMigrationAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic id reported for a migratable MediatR type usage.</summary>
    public const string DiagnosticId = "MQ100";

    /// <summary>Property-bag key holding the primary Mediarq type name.</summary>
    internal const string TypeKey = "type";

    /// <summary>Property-bag key holding the namespace that declares the primary Mediarq type.</summary>
    internal const string NamespaceKey = "namespace";

    /// <summary>Property-bag key holding the alternative Mediarq type name (when the MediatR type is ambiguous).</summary>
    internal const string AlternativeTypeKey = "altType";

    /// <summary>Property-bag key holding the namespace that declares the alternative Mediarq type.</summary>
    internal const string AlternativeNamespaceKey = "altNamespace";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "MediatR type has a Mediarq equivalent",
        messageFormat: "MediatR '{0}' maps to Mediarq '{1}'.{2}",
        category: "Mediarq.Migration",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Detects MediatR types that can be rewritten to their Mediarq equivalents to migrate off MediatR.",
        helpLinkUri: "https://github.com/rouffou/mediarq/blob/main/docs/guides/migrating-from-mediatr.md");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeName, SyntaxKind.IdentifierName, SyntaxKind.GenericName);
    }

    private static void AnalyzeName(SyntaxNodeAnalysisContext context)
    {
        var node = (SimpleNameSyntax)context.Node;

        // Resolve the name to a type symbol; tolerate binding errors mid-migration via candidate symbols.
        var symbolInfo = context.SemanticModel.GetSymbolInfo(node, context.CancellationToken);
        if (symbolInfo.Symbol is not INamedTypeSymbol type)
        {
            if (symbolInfo.CandidateSymbols.Length != 1 || symbolInfo.CandidateSymbols[0] is not INamedTypeSymbol candidate)
            {
                return;
            }

            type = candidate;
        }

        if (!IsInMediatRNamespace(type) || !MediatRTypeMap.TryGet(type.MetadataName, out var mapping))
        {
            return;
        }

        var properties = new Dictionary<string, string?>
        {
            [TypeKey] = mapping.Primary.Type,
            [NamespaceKey] = mapping.Primary.Namespace,
        };

        var mediarqDisplay = mapping.Primary.Type;
        if (mapping.Alternative is { } alternative)
        {
            properties[AlternativeTypeKey] = alternative.Type;
            properties[AlternativeNamespaceKey] = alternative.Namespace;
            mediarqDisplay = mapping.Primary.Type + " or " + alternative.Type;
        }

        var note = mapping.Primary.Note is { } n ? " " + n : string.Empty;

        context.ReportDiagnostic(Diagnostic.Create(
            Rule,
            node.GetLocation(),
            properties.ToImmutableDictionary(),
            type.Name,
            mediarqDisplay,
            note));
    }

    private static bool IsInMediatRNamespace(INamedTypeSymbol type)
    {
        var ns = type.ContainingNamespace;
        return ns is { IsGlobalNamespace: false, Name: MediatRTypeMap.MediatRNamespace }
            && ns.ContainingNamespace is { IsGlobalNamespace: true };
    }
}
