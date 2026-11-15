using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mediarq.Analyzers;

/// <summary>
/// Reports two concrete handler classes declared for the same request type
/// (<c>IRequestHandler&lt;TRequest, TResponse&gt;</c>, including through <c>ICommandHandler</c> /
/// <c>IQueryHandler</c>). A request has exactly one handler: with two, dispatch cannot pick one, and
/// <c>Mediarq.HealthChecks</c> only reports it at runtime.
/// </summary>
/// <remarks>
/// Only handlers declared in the compilation being analyzed are compared, and only closed request types
/// (an open-generic handler or request is skipped). Reported as a warning so that adding the analyzer
/// never breaks an existing build; raise the severity with an <c>.editorconfig</c> entry
/// (<c>dotnet_diagnostic.MQ203.severity = error</c>) to make it a compile error.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DuplicateHandlerAnalyzer : DiagnosticAnalyzer
{
    /// <summary>The diagnostic id reported when a request type has more than one handler.</summary>
    public const string DiagnosticId = "MQ203";

    private const string HandlerInterfaceName = "IRequestHandler";
    private const string HandlerInterfaceNamespace = "Mediarq.Core.Common.Requests.Abstraction";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Request has more than one handler",
        messageFormat: "'{0}' is a second handler for request '{1}', already handled by '{2}' -- a request must have exactly one handler",
        category: "Mediarq.Handlers",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Two handler classes implement the handler interface for the same request type. A request has exactly one handler, so dispatch cannot choose between them; Mediarq.HealthChecks would only report it at runtime.",
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
            var handlers = new ConcurrentBag<HandlerDeclaration>();
            startContext.RegisterSymbolAction(symbolContext => CollectHandlers(symbolContext, handlers), SymbolKind.NamedType);
            startContext.RegisterCompilationEndAction(endContext => ReportDuplicates(endContext, handlers));
        });
    }

    private static void CollectHandlers(SymbolAnalysisContext context, ConcurrentBag<HandlerDeclaration> handlers)
    {
        var type = (INamedTypeSymbol)context.Symbol;
        if (type.TypeKind is not (TypeKind.Class or TypeKind.Struct) || type.IsAbstract || type.IsGenericType)
        {
            return;
        }

        foreach (var iface in type.AllInterfaces)
        {
            if (!IsRequestHandlerInterface(iface))
            {
                continue;
            }

            var requestType = iface.TypeArguments[0];
            if (requestType.TypeKind == TypeKind.TypeParameter || !IsClosed(requestType))
            {
                continue;
            }

            var location = type.Locations.FirstOrDefault(l => l.IsInSource) ?? Location.None;
            handlers.Add(new HandlerDeclaration(type, requestType, location));
        }
    }

    private static void ReportDuplicates(CompilationAnalysisContext context, ConcurrentBag<HandlerDeclaration> handlers)
    {
        foreach (var group in handlers.GroupBy(h => h.RequestType, SymbolEqualityComparer.Default))
        {
            var ordered = group
                .GroupBy(h => h.HandlerType, SymbolEqualityComparer.Default)
                .Select(g => g.First())
                .OrderBy(h => h.HandlerType.ToDisplayString(), System.StringComparer.Ordinal)
                .ToList();

            if (ordered.Count < 2)
            {
                continue;
            }

            var first = ordered[0];
            foreach (var duplicate in ordered.Skip(1))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Rule, duplicate.Location, duplicate.HandlerType.Name, duplicate.RequestType.Name, first.HandlerType.Name));
            }
        }
    }

    private static bool IsRequestHandlerInterface(INamedTypeSymbol iface)
        => iface.OriginalDefinition is { Name: HandlerInterfaceName, Arity: 2 } definition
            && definition.ContainingNamespace?.ToDisplayString() == HandlerInterfaceNamespace;

    private static bool IsClosed(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol named)
        {
            return named.TypeArguments.All(IsClosed);
        }

        return type.TypeKind != TypeKind.TypeParameter;
    }

    private sealed class HandlerDeclaration
    {
        public HandlerDeclaration(INamedTypeSymbol handlerType, ITypeSymbol requestType, Location location)
        {
            HandlerType = handlerType;
            RequestType = requestType;
            Location = location;
        }

        public INamedTypeSymbol HandlerType { get; }
        public ITypeSymbol RequestType { get; }
        public Location Location { get; }
    }
}
