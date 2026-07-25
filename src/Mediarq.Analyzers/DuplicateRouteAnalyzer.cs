using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mediarq.Analyzers;

/// <summary>
/// Reports two Mediarq-attributed request types (<c>[MediarqGet]</c>/<c>[MediarqPost]</c>/<c>[MediarqPut]</c>/
/// <c>[MediarqPatch]</c>/<c>[MediarqDelete]</c>) declaring the same <c>(HTTP method, route pattern)</c> pair.
/// <c>Mediarq.AspNetCore.MediarqEndpointRouteBuilderExtensions.MapMediarq</c> maps every attributed type
/// with zero uniqueness check, so two colliding types only surface as an ambiguous-match error at
/// ASP.NET Core's routing time, on the first request that matches the shared pattern — never at build time.
/// </summary>
/// <remarks>
/// This is a syntactic/string check over the attribute's literal pattern argument, mirroring
/// <see cref="CaptiveDependencyAnalyzer"/>'s by-name/by-namespace matching: it does not parse route
/// pattern syntax (parameter names, constraints, ...), so patterns that are equivalent but written
/// differently (e.g. differing only by a route constraint) are not detected. It only flags an exact,
/// literal match of both the HTTP method and the pattern text.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DuplicateRouteAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic id reported for a duplicate Mediarq route.</summary>
    public const string DiagnosticId = "MQ202";

    private const string RouteAttributesNamespace = "Mediarq.AspNetCore";

    private static readonly (string AttributeName, string Method)[] RouteAttributes =
    [
        ("MediarqGetAttribute", "GET"),
        ("MediarqPostAttribute", "POST"),
        ("MediarqPutAttribute", "PUT"),
        ("MediarqPatchAttribute", "PATCH"),
        ("MediarqDeleteAttribute", "DELETE"),
    ];

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Duplicate Mediarq route",
        messageFormat: "'{0}' declares the same route as '{1}' ({2} {3}) -- ASP.NET Core will not be able to disambiguate them at request time",
        category: "Mediarq.Routing",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Two Mediarq-attributed request types declare the same (HTTP method, route pattern) pair. MapMediarq() maps both with no uniqueness check, so the collision only surfaces as ASP.NET Core's ambiguous-match error on the first request that matches the pattern, never at build time.",
        helpLinkUri: "https://github.com/rouffou/mediarq/blob/main/docs/guides/troubleshooting.md",
        customTags: WellKnownDiagnosticTags.CompilationEnd);

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(startContext =>
        {
            var routes = new ConcurrentBag<RouteDeclaration>();
            startContext.RegisterSymbolAction(symbolContext => CollectRoutes(symbolContext, routes), SymbolKind.NamedType);
            startContext.RegisterCompilationEndAction(endContext => ReportDuplicates(endContext, routes));
        });
    }

    private static void CollectRoutes(SymbolAnalysisContext context, ConcurrentBag<RouteDeclaration> routes)
    {
        var type = (INamedTypeSymbol)context.Symbol;
        if (type.TypeKind is not (TypeKind.Class or TypeKind.Struct))
        {
            return;
        }

        foreach (var attribute in type.GetAttributes())
        {
            var attributeClass = attribute.AttributeClass;
            if (attributeClass is null || attributeClass.ContainingNamespace?.ToDisplayString() != RouteAttributesNamespace)
            {
                continue;
            }

            var method = RouteAttributes.FirstOrDefault(r => r.AttributeName == attributeClass.Name).Method;
            if (method is null)
            {
                continue;
            }

            if (attribute.ConstructorArguments.Length == 0 || attribute.ConstructorArguments[0].Value is not string pattern)
            {
                continue;
            }

            var location = attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation()
                ?? type.Locations.FirstOrDefault()
                ?? Location.None;

            routes.Add(new RouteDeclaration(type, method, pattern, location));
        }
    }

    private static void ReportDuplicates(CompilationAnalysisContext context, ConcurrentBag<RouteDeclaration> routes)
    {
        foreach (var group in routes.GroupBy(r => (r.Method, r.Pattern)))
        {
            var ordered = group
                .OrderBy(r => r.Type.ToDisplayString(), System.StringComparer.Ordinal)
                .ToList();

            if (ordered.Select(r => r.Type).Distinct(SymbolEqualityComparer.Default).Count() < 2)
            {
                continue;
            }

            var first = ordered[0];
            foreach (var duplicate in ordered.Skip(1))
            {
                if (SymbolEqualityComparer.Default.Equals(duplicate.Type, first.Type))
                {
                    continue;
                }

                context.ReportDiagnostic(Diagnostic.Create(
                    Rule, duplicate.Location, duplicate.Type.Name, first.Type.Name, duplicate.Method, duplicate.Pattern));
            }
        }
    }

    private sealed class RouteDeclaration
    {
        public RouteDeclaration(INamedTypeSymbol type, string method, string pattern, Location location)
        {
            Type = type;
            Method = method;
            Pattern = pattern;
            Location = location;
        }

        public INamedTypeSymbol Type { get; }
        public string Method { get; }
        public string Pattern { get; }
        public Location Location { get; }
    }
}
