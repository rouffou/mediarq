using System.Threading.Channels;

namespace Mediarq.Deferred;

/// <summary>Configures the queue backing <see cref="IDeferredDispatcher"/>.</summary>
public sealed class DeferredDispatchOptions
{
    /// <summary>
    /// The queue's maximum item count, or <see langword="null"/> for an unbounded channel (the default).
    /// A bounded channel applies backpressure to <see cref="IDeferredDispatcher.SendLaterAsync{TCommand}"/>/
    /// <see cref="IDeferredDispatcher.PublishLaterAsync{TNotification}"/> per <see cref="FullMode"/> once full.
    /// </summary>
    public int? Capacity { get; set; }

    /// <summary>How the queue behaves once <see cref="Capacity"/> is reached. Ignored when unbounded.</summary>
    public BoundedChannelFullMode FullMode { get; set; } = BoundedChannelFullMode.Wait;
}
