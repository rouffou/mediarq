using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mediarq.Analyzers;

/// <summary>
/// Rewrites a MediatR type flagged by <see cref="MediatRMigrationAnalyzer"/> to its Mediarq equivalent,
/// adding the Mediarq namespace <c>using</c>. When the MediatR type is ambiguous (e.g.
/// <c>IRequest&lt;T&gt;</c>) it offers one fix per candidate (<c>ICommand&lt;T&gt;</c> / <c>IQuery&lt;T&gt;</c>).
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MediatRMigrationCodeFixProvider))]
[Shared]
public sealed class MediatRMigrationCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(MediatRMigrationAnalyzer.DiagnosticId);

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            if (root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is not SimpleNameSyntax name)
            {
                continue;
            }

            RegisterReplacement(context, diagnostic, name,
                Property(diagnostic, MediatRMigrationAnalyzer.TypeKey),
                Property(diagnostic, MediatRMigrationAnalyzer.NamespaceKey));

            RegisterReplacement(context, diagnostic, name,
                Property(diagnostic, MediatRMigrationAnalyzer.AlternativeTypeKey),
                Property(diagnostic, MediatRMigrationAnalyzer.AlternativeNamespaceKey));
        }
    }

    private static string? Property(Diagnostic diagnostic, string key)
        => diagnostic.Properties.TryGetValue(key, out var value) ? value : null;

    private static void RegisterReplacement(CodeFixContext context, Diagnostic diagnostic, SimpleNameSyntax name, string? type, string? @namespace)
    {
        if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(@namespace))
        {
            return;
        }

        var title = $"Convert to Mediarq '{type!}'";
        context.RegisterCodeFix(
            CodeAction.Create(
                title,
                ct => ApplyAsync(context.Document, name, type!, @namespace!, ct),
                equivalenceKey: type),
            diagnostic);
    }

    private static async Task<Document> ApplyAsync(Document document, SimpleNameSyntax name, string type, string @namespace, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        // Build the replacement name, preserving generic type arguments and surrounding trivia.
        SimpleNameSyntax replacement = name is GenericNameSyntax generic
            ? SyntaxFactory.GenericName(SyntaxFactory.Identifier(type)).WithTypeArgumentList(generic.TypeArgumentList)
            : SyntaxFactory.IdentifierName(type);

        // If the usage was fully qualified (MediatR.IRequest), replace the whole qualified name.
        SyntaxNode nodeToReplace = name.Parent is QualifiedNameSyntax qualified && qualified.Right == name
            ? qualified
            : name;

        var newRoot = root.ReplaceNode(nodeToReplace, replacement.WithTriviaFrom(nodeToReplace));
        newRoot = AddUsingIfMissing(newRoot, @namespace);

        return document.WithSyntaxRoot(newRoot);
    }

    private static SyntaxNode AddUsingIfMissing(SyntaxNode root, string @namespace)
    {
        if (root is not CompilationUnitSyntax compilationUnit)
        {
            return root;
        }

        if (compilationUnit.Usings.Any(u => u.Name?.ToString() == @namespace))
        {
            return root;
        }

        var directive = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(@namespace))
            .NormalizeWhitespace()
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        return compilationUnit.AddUsings(directive);
    }
}
