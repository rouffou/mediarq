using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Mediarq.SourceGenerators;

/// <summary>
/// Incremental source generator that discovers, at compile time, every Mediarq handler, pipeline
/// behavior and validator declared in the current assembly and emits an internal
/// <c>services.AddMediarqHandlers()</c> extension that registers them explicitly.
/// This removes the runtime, reflection-based assembly scan and is trimming/AOT friendly.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class MediarqRegistrationGenerator : IIncrementalGenerator
{
    private static readonly SymbolDisplayFormat FullyQualified = SymbolDisplayFormat.FullyQualifiedFormat;

    private static readonly DiagnosticDescriptor MultipleHandlers = new(
        id: "MQ001",
        title: "Multiple request handlers for the same request",
        messageFormat: "Multiple handlers are registered for '{0}'. Mediarq dispatches each request to a single handler.",
        category: "Mediarq",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var registrations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax { BaseList: not null } or RecordDeclarationSyntax { BaseList: not null },
                transform: static (ctx, ct) => Transform(ctx, ct))
            .Where(static models => models.Length > 0)
            .SelectMany(static (models, _) => models)
            .Collect()
            .Select(static (items, _) => new EquatableArray<HandlerRegistration>(items.Distinct().ToImmutableArray()));

        context.RegisterSourceOutput(registrations, static (spc, regs) => Emit(spc, regs));
    }

    private static EquatableArray<HandlerRegistration> Transform(GeneratorSyntaxContext ctx, CancellationToken cancellationToken)
    {
        if (ctx.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)ctx.Node, cancellationToken) is not INamedTypeSymbol type)
        {
            return default;
        }

        if (type.TypeKind != TypeKind.Class || type.IsAbstract || type.IsStatic)
        {
            return default;
        }

        if (type.DeclaredAccessibility != Accessibility.Public && type.DeclaredAccessibility != Accessibility.Internal)
        {
            return default;
        }

        var ns = type.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        if (ns == "Mediarq.Core" || ns.StartsWith("Mediarq.Core."))
        {
            return default;
        }

        var list = new List<HandlerRegistration>();
        foreach (var iface in type.AllInterfaces)
        {
            var definition = iface.OriginalDefinition;
            var key = definition.ContainingNamespace.ToDisplayString() + "." + definition.MetadataName;
            if (!IsTarget(key))
            {
                continue;
            }

            bool isRequestHandler = key == "Mediarq.Core.Common.Requests.Abstraction.IRequestHandler`2";

            if (type.IsGenericType)
            {
                list.Add(new HandlerRegistration(
                    definition.ConstructUnboundGenericType().ToDisplayString(FullyQualified),
                    type.ConstructUnboundGenericType().ToDisplayString(FullyQualified),
                    IsOpenGeneric: true,
                    IsRequestHandler: isRequestHandler));
            }
            else
            {
                list.Add(new HandlerRegistration(
                    iface.ToDisplayString(FullyQualified),
                    type.ToDisplayString(FullyQualified),
                    IsOpenGeneric: false,
                    IsRequestHandler: isRequestHandler));
            }
        }

        return new EquatableArray<HandlerRegistration>(list.ToImmutableArray());
    }

    private static bool IsTarget(string key) =>
        key == "Mediarq.Core.Common.Requests.Abstraction.IRequestHandler`2" ||
        key == "Mediarq.Core.Common.Requests.Notifications.INotificationHandler`1" ||
        key == "Mediarq.Core.Common.Pipeline.IPipelineBehavior`2" ||
        key == "Mediarq.Core.Common.Requests.Validators.IValidator`1";

    private static void Emit(SourceProductionContext context, EquatableArray<HandlerRegistration> registrations)
    {
        // Diagnose multiple handlers registered for the same request (a request handler must be unique).
        foreach (var group in registrations.Where(r => r.IsRequestHandler).GroupBy(r => r.ServiceType))
        {
            if (group.Select(r => r.ImplType).Distinct().Count() > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(MultipleHandlers, Location.None, group.Key));
            }
        }

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine();
        sb.AppendLine("namespace Mediarq.Extensions");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>Compile-time generated Mediarq handler, behavior and validator registrations.</summary>");
        sb.AppendLine("    internal static class MediarqGeneratedRegistration");
        sb.AppendLine("    {");
        sb.AppendLine("        /// <summary>Registers every Mediarq handler, behavior and validator discovered at compile time.</summary>");
        sb.AppendLine("        internal static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddMediarqHandlers(this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)");
        sb.AppendLine("        {");

        foreach (var reg in registrations.OrderBy(r => r.ServiceType + "|" + r.ImplType, System.StringComparer.Ordinal))
        {
            if (reg.IsOpenGeneric)
            {
                sb.Append("            services.AddScoped(typeof(").Append(reg.ServiceType).Append("), typeof(").Append(reg.ImplType).AppendLine("));");
            }
            else
            {
                sb.Append("            services.AddScoped<").Append(reg.ServiceType).Append(", ").Append(reg.ImplType).AppendLine(">();");
            }
        }

        sb.AppendLine("            return services;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource("MediarqGeneratedRegistration.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}

/// <summary>One discovered DI registration to emit.</summary>
internal readonly record struct HandlerRegistration(string ServiceType, string ImplType, bool IsOpenGeneric, bool IsRequestHandler);
