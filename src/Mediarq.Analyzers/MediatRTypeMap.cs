using System.Collections.Generic;

namespace Mediarq.Analyzers;

/// <summary>
/// One MediatR-to-Mediarq replacement: the Mediarq type name, the namespace that declares it, and an
/// optional note about why the migration is not purely mechanical.
/// </summary>
internal readonly struct MediarqReplacement
{
    public MediarqReplacement(string type, string @namespace, string? note = null)
    {
        Type = type;
        Namespace = @namespace;
        Note = note;
    }

    /// <summary>The Mediarq type to use instead (simple name, e.g. <c>ICommand</c>).</summary>
    public string Type { get; }

    /// <summary>The namespace that declares <see cref="Type"/> (added as a <c>using</c> by the code fix).</summary>
    public string Namespace { get; }

    /// <summary>An optional caveat surfaced to the user (e.g. a signature difference); <see langword="null"/> when the swap is 1:1.</summary>
    public string? Note { get; }
}

/// <summary>
/// One mapping entry: the primary Mediarq replacement and, when the MediatR type is ambiguous, an
/// alternative (e.g. MediatR's <c>IRequest&lt;T&gt;</c> can become either <c>ICommand&lt;T&gt;</c> or
/// <c>IQuery&lt;T&gt;</c>).
/// </summary>
internal readonly struct MediatRMapping
{
    public MediatRMapping(MediarqReplacement primary, MediarqReplacement? alternative = null)
    {
        Primary = primary;
        Alternative = alternative;
    }

    public MediarqReplacement Primary { get; }

    public MediarqReplacement? Alternative { get; }
}

/// <summary>
/// The canonical MediatR → Mediarq type map, keyed by the MediatR type's metadata name (so generic
/// arities are explicit, e.g. <c>IRequest`1</c>). Shared by the analyzer and the code fix.
/// </summary>
internal static class MediatRTypeMap
{
    /// <summary>The namespace MediatR declares its public abstractions in.</summary>
    public const string MediatRNamespace = "MediatR";

    private const string CommandNs = "Mediarq.Core.Common.Requests.Command";
    private const string QueryNs = "Mediarq.Core.Common.Requests.Query";
    private const string AbstractionNs = "Mediarq.Core.Common.Requests.Abstraction";
    private const string NotificationsNs = "Mediarq.Core.Common.Requests.Notifications";
    private const string StreamingNs = "Mediarq.Core.Common.Requests.Streaming";
    private const string PipelineNs = "Mediarq.Core.Common.Pipeline";
    private const string MediatorsNs = "Mediarq.Core.Mediators";

    private const string PipelineNote =
        "Mediarq's IPipelineBehavior has a different Handle signature: the request is exposed via an " +
        "IMutableRequestContext and the continuation is a Func<Task<TResponse>> (not RequestHandlerDelegate).";

    private static readonly Dictionary<string, MediatRMapping> Map = new()
    {
        // Requests — void IRequest becomes a no-result command; IRequest<T> is ambiguous (command or query).
        ["IRequest"] = new(new MediarqReplacement("ICommand", CommandNs)),
        ["IRequest`1"] = new(
            new MediarqReplacement("ICommand", CommandNs),
            new MediarqReplacement("IQuery", QueryNs)),

        // Handlers — the Mediarq names match; only the namespace changes.
        ["IRequestHandler`2"] = new(new MediarqReplacement("IRequestHandler", AbstractionNs)),
        ["IRequestHandler`1"] = new(new MediarqReplacement("IRequestHandler", AbstractionNs)),

        // Notifications — 1:1.
        ["INotification"] = new(new MediarqReplacement("INotification", NotificationsNs)),
        ["INotificationHandler`1"] = new(new MediarqReplacement("INotificationHandler", NotificationsNs)),

        // Streaming — 1:1.
        ["IStreamRequest`1"] = new(new MediarqReplacement("IStreamRequest", StreamingNs)),
        ["IStreamRequestHandler`2"] = new(new MediarqReplacement("IStreamRequestHandler", StreamingNs)),

        // Pipeline — same name, but the Handle signature differs (flagged via the note).
        ["IPipelineBehavior`2"] = new(new MediarqReplacement("IPipelineBehavior", PipelineNs, PipelineNote)),

        // Mediator entry points — 1:1.
        ["ISender"] = new(new MediarqReplacement("ISender", MediatorsNs)),
        ["IPublisher"] = new(new MediarqReplacement("IPublisher", MediatorsNs)),
        ["IMediator"] = new(new MediarqReplacement("IMediator", MediatorsNs)),
    };

    /// <summary>Looks up the Mediarq mapping for a MediatR type by its metadata name.</summary>
    public static bool TryGet(string metadataName, out MediatRMapping mapping)
        => Map.TryGetValue(metadataName, out mapping);
}
