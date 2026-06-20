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

    private static readonly DiagnosticDescriptor MissingHandler = new(
        id: "MQ002",
        title: "No request handler found for a command or query",
        messageFormat: "No IRequestHandler was found in this assembly for '{0}'. Define a handler, or ignore this if the handler lives in another assembly.",
        category: "Mediarq",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor OrphanValidator = new(
        id: "MQ003",
        title: "Validator targets a type that is never validated",
        messageFormat: "Validator for '{0}' will never run: Mediarq only validates requests (ICommandOrQuery<T>) and notifications (INotification). Make the validated type a request/notification, or remove the validator.",
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

        // Optional: make the generated AddMediarqHandlers() public (default internal) so it can be
        // called from another assembly. Set <MediarqGeneratedAccessibility>public</...> in the consumer.
        var isPublic = context.AnalyzerConfigOptionsProvider.Select(static (provider, _) =>
            provider.GlobalOptions.TryGetValue("build_property.MediarqGeneratedAccessibility", out var value) &&
            string.Equals(value, "public", System.StringComparison.OrdinalIgnoreCase));

        // Optional: the namespace of the generated registration class. Set <MediarqGeneratedNamespace>...</...>
        // in the consumer; defaults to "Mediarq.Extensions".
        var generatedNamespace = context.AnalyzerConfigOptionsProvider.Select(static (provider, _) =>
            provider.GlobalOptions.TryGetValue("build_property.MediarqGeneratedNamespace", out var value) &&
            !string.IsNullOrWhiteSpace(value)
                ? value!.Trim()
                : "Mediarq.Extensions");

        var options = isPublic.Combine(generatedNamespace);

        context.RegisterSourceOutput(registrations.Combine(options),
            static (spc, pair) => Emit(spc, pair.Left, pair.Right.Left, pair.Right.Right));
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

        var lifetime = ReadLifetime(type);

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
                    ResponseIsGenericResult: false,
                    Lifetime: lifetime,
                    IsOrphanValidator: false));
            }
            else
            {
                string? requestType = null;
                string? responseType = null;
                string? notificationType = null;
                bool responseIsGenericResult = false;
                bool isOrphanValidator = false;

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
                else if (kind == HandlerKind.Validator && iface.TypeArguments.Length == 1)
                {
                    // A validator only runs when its target is a mediated type (request or notification).
                    // Otherwise it is orphaned — flag it for MQ003. RequestType carries the validated type
                    // for the diagnostic message.
                    var validatedType = iface.TypeArguments[0];
                    requestType = validatedType.ToDisplayString(FullyQualified);
                    isOrphanValidator = !IsMediatedType(validatedType);
                }

                list.Add(new HandlerRegistration(
                    iface.ToDisplayString(FullyQualified),
                    type.ToDisplayString(FullyQualified),
                    IsOpenGeneric: false,
                    Kind: kind,
                    RequestType: requestType,
                    ResponseType: responseType,
                    NotificationType: notificationType,
                    ResponseIsGenericResult: responseIsGenericResult,
                    Lifetime: lifetime,
                    IsOrphanValidator: isOrphanValidator));
            }
        }

        // Not a handler/behavior/validator but a declared command/query: track it so a missing
        // handler can be diagnosed (MQ002).
        if (list.Count == 0 && !type.IsGenericType && ImplementsCommandOrQuery(type))
        {
            var requestFqn = type.ToDisplayString(FullyQualified);
            list.Add(new HandlerRegistration(
                requestFqn,
                string.Empty,
                IsOpenGeneric: false,
                Kind: HandlerKind.Request,
                RequestType: requestFqn,
                ResponseType: null,
                NotificationType: null,
                ResponseIsGenericResult: false,
                Lifetime: "Scoped",
                IsOrphanValidator: false));
        }

        return new EquatableArray<HandlerRegistration>(list.ToImmutableArray());
    }

    private static bool ImplementsCommandOrQuery(INamedTypeSymbol type)
    {
        foreach (var iface in type.AllInterfaces)
        {
            var definition = iface.OriginalDefinition;
            if (definition.MetadataName == "ICommandOrQuery`1" &&
                definition.ContainingNamespace?.ToDisplayString() == "Mediarq.Core.Common.Requests.Abstraction")
            {
                return true;
            }
        }

        return false;
    }

    // True when the type takes part in the Mediarq pipeline as a request (ICommandOrQuery<T>) or a
    // notification (INotification) — the only places a validator is ever invoked.
    private static bool IsMediatedType(ITypeSymbol type)
    {
        if (type is ITypeParameterSymbol)
        {
            // Open-generic validated type (e.g. IValidator<T>): cannot tell, do not flag.
            return true;
        }

        foreach (var iface in type.AllInterfaces)
        {
            var definition = iface.OriginalDefinition;
            var ns = definition.ContainingNamespace?.ToDisplayString();
            if (definition.MetadataName == "ICommandOrQuery`1" && ns == "Mediarq.Core.Common.Requests.Abstraction")
            {
                return true;
            }

            if (definition.MetadataName == "INotification" && ns == "Mediarq.Core.Common.Requests.Notifications")
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsGenericResult(ITypeSymbol responseType) =>
        responseType is INamedTypeSymbol named &&
        named.IsGenericType &&
        named.OriginalDefinition.MetadataName == "Result`1" &&
        (named.OriginalDefinition.ContainingNamespace?.ToDisplayString() == "Mediarq.Core.Common.Results");

    // Reads the [RegisterHandler(ServiceLifetime)] attribute, defaulting to Scoped.
    // ServiceLifetime: Singleton = 0, Scoped = 1, Transient = 2.
    private static string ReadLifetime(INamedTypeSymbol type)
    {
        foreach (var attribute in type.GetAttributes())
        {
            var attributeClass = attribute.AttributeClass;
            if (attributeClass is null)
            {
                continue;
            }

            if (attributeClass.Name == "RegisterHandlerAttribute" &&
                attributeClass.ContainingNamespace?.ToDisplayString() == "Mediarq.Core.Common.Registration")
            {
                if (attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is int value)
                {
                    return value switch
                    {
                        0 => "Singleton",
                        2 => "Transient",
                        _ => "Scoped",
                    };
                }

                return "Scoped";
            }
        }

        return "Scoped";
    }

    private static bool IsTarget(string key) => KindOf(key) != HandlerKind.Other;

    private static HandlerKind KindOf(string key) => key switch
    {
        "Mediarq.Core.Common.Requests.Abstraction.IRequestHandler`2" => HandlerKind.RequestHandler,
        "Mediarq.Core.Common.Requests.Notifications.INotificationHandler`1" => HandlerKind.NotificationHandler,
        "Mediarq.Core.Common.Pipeline.IPipelineBehavior`2" => HandlerKind.Behavior,
        "Mediarq.Core.Common.Pipeline.IStreamPipelineBehavior`2" => HandlerKind.Behavior,
        "Mediarq.Core.Common.Requests.Validators.IValidator`1" => HandlerKind.Validator,
        "Mediarq.Core.Common.Requests.Exceptions.IRequestExceptionHandler`2" => HandlerKind.ExceptionHandler,
        "Mediarq.Core.Common.Requests.Streaming.IStreamRequestHandler`2" => HandlerKind.StreamHandler,
        // Pre/post-processors are registered like behaviors (plain AddScoped, no dispatch wrapper).
        "Mediarq.Core.Common.Requests.Processors.IRequestPreProcessor`1" => HandlerKind.Behavior,
        "Mediarq.Core.Common.Requests.Processors.IRequestPostProcessor`2" => HandlerKind.Behavior,
        _ => HandlerKind.Other,
    };

    private static void Emit(SourceProductionContext context, EquatableArray<HandlerRegistration> registrations, bool isPublic, string generatedNamespace)
    {
        var accessibility = isPublic ? "public" : "internal";

        // Diagnose multiple handlers registered for the same request (a request handler must be unique).
        foreach (var group in registrations.Where(r => r.Kind == HandlerKind.RequestHandler).GroupBy(r => r.ServiceType))
        {
            if (group.Select(r => r.ImplType).Distinct().Count() > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(MultipleHandlers, Location.None, group.Key));
            }
        }

        // Diagnose a declared command/query that has no handler in this assembly (MQ002, informational).
        var handledRequestTypes = new HashSet<string>(
            registrations.Where(r => r.Kind == HandlerKind.RequestHandler && r.RequestType != null).Select(r => r.RequestType!));

        foreach (var request in registrations.Where(r => r.Kind == HandlerKind.Request && r.RequestType != null))
        {
            if (!handledRequestTypes.Contains(request.RequestType!))
            {
                context.ReportDiagnostic(Diagnostic.Create(MissingHandler, Location.None, request.RequestType));
            }
        }

        // Diagnose a validator whose target is neither a request nor a notification (MQ003): it can
        // never be executed by the pipeline.
        foreach (var orphan in registrations.Where(r => r.Kind == HandlerKind.Validator && r.IsOrphanValidator && r.RequestType != null))
        {
            context.ReportDiagnostic(Diagnostic.Create(OrphanValidator, Location.None, orphan.RequestType));
        }

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine();
        sb.Append("namespace ").AppendLine(generatedNamespace);
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>Compile-time generated Mediarq handler, behavior and validator registrations.</summary>");
        sb.Append("    ").Append(accessibility).AppendLine(" static class MediarqGeneratedRegistration");
        sb.AppendLine("    {");
        sb.AppendLine("        /// <summary>Registers every Mediarq handler, behavior and validator discovered at compile time.</summary>");
        sb.Append("        ").Append(accessibility).AppendLine(" static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddMediarqHandlers(this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)");
        sb.AppendLine("        {");

        foreach (var reg in registrations.Where(r => r.Kind != HandlerKind.Request).OrderBy(r => r.ServiceType + "|" + r.ImplType, StringComparer.Ordinal))
        {
            if (reg.IsOpenGeneric)
            {
                sb.Append("            services.Add").Append(reg.Lifetime).Append("(typeof(").Append(reg.ServiceType).Append("), typeof(").Append(reg.ImplType).AppendLine("));");
            }
            else
            {
                sb.Append("            services.Add").Append(reg.Lifetime).Append('<').Append(reg.ServiceType).Append(", ").Append(reg.ImplType).AppendLine(">();");
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

    /// <summary>A declared command/query (implements ICommandOrQuery&lt;T&gt;), tracked only to diagnose a missing handler.</summary>
    Request,
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
    bool ResponseIsGenericResult,
    string Lifetime,
    bool IsOrphanValidator);
