using System;
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
/// It also pre-populates a <c>MediarqWrapperRegistry</c> with strongly-typed dispatch wrappers, so
/// the mediator never needs <c>Activator.CreateInstance</c>/<c>MakeGenericType</c> at runtime.
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

            var kind = KindOf(key);

            if (type.IsGenericType)
            {
                // Open-generic handlers cannot be registered as closed wrappers; the reflective
                // fallback handles them. We still emit the open-generic DI registration.
                list.Add(new HandlerRegistration(
                    definition.ConstructUnboundGenericType().ToDisplayString(FullyQualified),
                    type.ConstructUnboundGenericType().ToDisplayString(FullyQualified),
                    IsOpenGeneric: true,
                    Kind: kind,
                    RequestType: null,
                    ResponseType: null,
                    NotificationType: null,
                    ResponseIsGenericResult: false));
            }
            else
            {
                string? requestType = null;
                string? responseType = null;
                string? notificationType = null;
                bool responseIsGenericResult = false;

                if (kind == HandlerKind.RequestHandler && iface.TypeArguments.Length == 2)
                {
                    requestType = iface.TypeArguments[0].ToDisplayString(FullyQualified);
                    responseType = iface.TypeArguments[1].ToDisplayString(FullyQualified);
                    responseIsGenericResult = IsGenericResult(iface.TypeArguments[1]);
                }
                else if (kind == HandlerKind.StreamHandler && iface.TypeArguments.Length == 2)
                {
                    requestType = iface.TypeArguments[0].ToDisplayString(FullyQualified);
                    responseType = iface.TypeArguments[1].ToDisplayString(FullyQualified);
                }
                else if (kind == HandlerKind.NotificationHandler && iface.TypeArguments.Length == 1)
                {
                    notificationType = iface.TypeArguments[0].ToDisplayString(FullyQualified);
                }

                list.Add(new HandlerRegistration(
                    iface.ToDisplayString(FullyQualified),
                    type.ToDisplayString(FullyQualified),
                    IsOpenGeneric: false,
                    Kind: kind,
                    RequestType: requestType,
                    ResponseType: responseType,
                    NotificationType: notificationType,
                    ResponseIsGenericResult: responseIsGenericResult));
            }
        }

        return new EquatableArray<HandlerRegistration>(list.ToImmutableArray());
    }

    private static bool IsGenericResult(ITypeSymbol responseType) =>
        responseType is INamedTypeSymbol named &&
        named.IsGenericType &&
        named.OriginalDefinition.MetadataName == "Result`1" &&
        (named.OriginalDefinition.ContainingNamespace?.ToDisplayString() == "Mediarq.Core.Common.Results");

    private static bool IsTarget(string key) => KindOf(key) != HandlerKind.Other;

    private static HandlerKind KindOf(string key) => key switch
    {
        "Mediarq.Core.Common.Requests.Abstraction.IRequestHandler`2" => HandlerKind.RequestHandler,
        "Mediarq.Core.Common.Requests.Notifications.INotificationHandler`1" => HandlerKind.NotificationHandler,
        "Mediarq.Core.Common.Pipeline.IPipelineBehavior`2" => HandlerKind.Behavior,
        "Mediarq.Core.Common.Requests.Validators.IValidator`1" => HandlerKind.Validator,
        "Mediarq.Core.Common.Requests.Exceptions.IRequestExceptionHandler`2" => HandlerKind.ExceptionHandler,
        "Mediarq.Core.Common.Requests.Streaming.IStreamRequestHandler`2" => HandlerKind.StreamHandler,
        _ => HandlerKind.Other,
    };

    private static void Emit(SourceProductionContext context, EquatableArray<HandlerRegistration> registrations)
    {
        // Diagnose multiple handlers registered for the same request (a request handler must be unique).
        foreach (var group in registrations.Where(r => r.Kind == HandlerKind.RequestHandler).GroupBy(r => r.ServiceType))
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

        foreach (var reg in registrations.OrderBy(r => r.ServiceType + "|" + r.ImplType, StringComparer.Ordinal))
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

        EmitWrapperRegistry(sb, registrations);

        sb.AppendLine("            return services;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource("MediarqGeneratedRegistration.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static void EmitWrapperRegistry(StringBuilder sb, EquatableArray<HandlerRegistration> registrations)
    {
        var requestWrappers = registrations
            .Where(r => r.Kind == HandlerKind.RequestHandler && !r.IsOpenGeneric && r.RequestType != null && r.ResponseType != null)
            .Select(r => r.RequestType + "|" + r.ResponseType)
            .Distinct()
            .OrderBy(s => s, StringComparer.Ordinal)
            .ToList();

        var notificationWrappers = registrations
            .Where(r => r.Kind == HandlerKind.NotificationHandler && !r.IsOpenGeneric && r.NotificationType != null)
            .Select(r => r.NotificationType!)
            .Distinct()
            .OrderBy(s => s, StringComparer.Ordinal)
            .ToList();

        var streamWrappers = registrations
            .Where(r => r.Kind == HandlerKind.StreamHandler && !r.IsOpenGeneric && r.RequestType != null && r.ResponseType != null)
            .Select(r => r.RequestType + "|" + r.ResponseType)
            .Distinct()
            .OrderBy(s => s, StringComparer.Ordinal)
            .ToList();

        // Result<T> responses get a reflection-free validation-failure factory so ValidationBehavior
        // can short-circuit without dynamic code on AOT.
        var resultResponses = registrations
            .Where(r => r.Kind == HandlerKind.RequestHandler && !r.IsOpenGeneric && r.ResponseIsGenericResult && r.ResponseType != null)
            .Select(r => r.ResponseType!)
            .Distinct()
            .OrderBy(s => s, StringComparer.Ordinal)
            .ToList();

        if (requestWrappers.Count == 0 && notificationWrappers.Count == 0 && streamWrappers.Count == 0)
        {
            return;
        }

        sb.AppendLine("            var registry = new global::Mediarq.Core.Mediators.MediarqWrapperRegistry();");

        foreach (var pair in requestWrappers)
        {
            var parts = pair.Split('|');
            sb.Append("            registry.Add<").Append(parts[0]).Append(", ").Append(parts[1]).AppendLine(">();");
        }

        foreach (var notification in notificationWrappers)
        {
            sb.Append("            registry.AddNotification<").Append(notification).AppendLine(">();");
        }

        foreach (var pair in streamWrappers)
        {
            var parts = pair.Split('|');
            sb.Append("            registry.AddStream<").Append(parts[0]).Append(", ").Append(parts[1]).AppendLine(">();");
        }

        sb.AppendLine("            services.AddSingleton(registry);");

        foreach (var response in resultResponses)
        {
            sb.Append("            global::Mediarq.Core.Common.Pipeline.Behaviors.ValidationFailureRegistry.Register<")
              .Append(response)
              .Append(">(static __error => ")
              .Append(response)
              .AppendLine(".ValidationFailure(__error));");
        }
    }
}

/// <summary>The kind of Mediarq abstraction a discovered registration implements.</summary>
internal enum HandlerKind
{
    Other,
    RequestHandler,
    NotificationHandler,
    Behavior,
    Validator,
    ExceptionHandler,
    StreamHandler,
}

/// <summary>One discovered DI registration to emit.</summary>
internal readonly record struct HandlerRegistration(
    string ServiceType,
    string ImplType,
    bool IsOpenGeneric,
    HandlerKind Kind,
    string? RequestType,
    string? ResponseType,
    string? NotificationType,
    bool ResponseIsGenericResult);
