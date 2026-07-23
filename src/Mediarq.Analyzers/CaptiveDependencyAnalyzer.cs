using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mediarq.Analyzers;

/// <summary>
/// Reports a Mediarq handler/behavior/validator explicitly registered as
/// <c>ServiceLifetime.Singleton</c> (via <c>[RegisterHandler(ServiceLifetime.Singleton)]</c>) whose
/// constructor depends on a shorter-lived type — a <c>DbContext</c>, or another Mediarq type explicitly
/// registered as <c>Scoped</c>/<c>Transient</c>. The dependency instance would be resolved once and
/// captured for the lifetime of the app instead of per scope/request: the classic "captive dependency"
/// bug (stale data, a disposed/thread-unsafe <c>DbContext</c> shared across requests, ...).
/// </summary>
/// <remarks>
/// Only types with an <em>explicit</em> lifetime marker are compared, on both sides: this analyzer has
/// no visibility into a consumer's arbitrary <c>services.AddScoped&lt;T&gt;()</c> calls outside the
/// Mediarq registration model, so it stays silent rather than guess and risk false positives.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CaptiveDependencyAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic id reported for a captive-dependency risk.</summary>
    public const string DiagnosticId = "MQ200";

    private const string RegisterHandlerAttributeName = "RegisterHandlerAttribute";
    private const string RegisterHandlerNamespace = "Mediarq.Core.Common.Registration";
    private const string DbContextTypeName = "DbContext";
    private const string DbContextNamespace = "Microsoft.EntityFrameworkCore";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Singleton depends on a shorter-lived type (captive dependency)",
        messageFormat: "'{0}' is registered as Singleton but its constructor depends on '{1}', {2} — the instance would be captured for the app's lifetime instead of per scope",
        category: "Mediarq.Lifetime",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "A Singleton-lifetime Mediarq handler, behavior or validator that constructor-injects a Scoped or Transient dependency (or a DbContext) captures it for the lifetime of the app instead of resolving a fresh instance per scope — usually a bug.",
        helpLinkUri: "https://github.com/rouffou/mediarq/blob/main/docs/guides/troubleshooting.md#lifetime");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        var type = (INamedTypeSymbol)context.Symbol;
        if (type.TypeKind != TypeKind.Class || type.IsAbstract)
        {
            return;
        }

        if (!TryGetExplicitLifetime(type, out var lifetime) || lifetime != Lifetime.Singleton)
        {
            return;
        }

        foreach (var ctor in type.InstanceConstructors)
        {
            if (ctor.DeclaredAccessibility != Accessibility.Public)
            {
                continue;
            }

            foreach (var parameter in ctor.Parameters)
            {
                if (parameter.Type is not INamedTypeSymbol parameterType)
                {
                    continue;
                }

                if (IsDbContext(parameterType))
                {
                    Report(context, type, parameter, "which is a DbContext (scoped by convention)");
                    continue;
                }

                if (TryGetExplicitLifetime(parameterType, out var dependencyLifetime) && dependencyLifetime != Lifetime.Singleton)
                {
                    Report(context, type, parameter, "registered as " + dependencyLifetime);
                }
            }
        }
    }

    private static void Report(SymbolAnalysisContext context, INamedTypeSymbol type, IParameterSymbol parameter, string reason)
    {
        var location = parameter.Locations.FirstOrDefault() ?? type.Locations.FirstOrDefault() ?? Location.None;
        context.ReportDiagnostic(Diagnostic.Create(Rule, location, type.Name, parameter.Type.Name, reason));
    }

    private static bool IsDbContext(INamedTypeSymbol type)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (current.Name == DbContextTypeName && current.ContainingNamespace?.ToDisplayString() == DbContextNamespace)
            {
                return true;
            }
        }

        return false;
    }

    // Reads [RegisterHandler(ServiceLifetime)]; returns false when the attribute is absent (default
    // Scoped, but not an *explicit* signal we can safely compare against).
    private static bool TryGetExplicitLifetime(INamedTypeSymbol type, out string lifetime)
    {
        foreach (var attribute in type.GetAttributes())
        {
            var attributeClass = attribute.AttributeClass;
            if (attributeClass is null)
            {
                continue;
            }

            if (attributeClass.Name != RegisterHandlerAttributeName ||
                attributeClass.ContainingNamespace?.ToDisplayString() != RegisterHandlerNamespace)
            {
                continue;
            }

            lifetime = attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is int value
                ? value switch
                {
                    0 => Lifetime.Singleton,
                    2 => Lifetime.Transient,
                    _ => Lifetime.Scoped,
                }
                : Lifetime.Scoped;
            return true;
        }

        lifetime = string.Empty;
        return false;
    }

    private static class Lifetime
    {
        public const string Singleton = "Singleton";
        public const string Scoped = "Scoped";
        public const string Transient = "Transient";
    }
}
