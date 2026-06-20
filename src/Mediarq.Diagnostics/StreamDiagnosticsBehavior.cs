using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Streaming;

namespace Mediarq.Diagnostics;

/// <summary>
/// Stream pipeline behavior that records an <see cref="Activity"/> and metrics (item count + duration)
/// for every streamed request, mirroring <see cref="DiagnosticsBehavior{TRequest, TResponse}"/> for <c>Send</c>.
/// </summary>
/// <typeparam name="TRequest">The stream-request type.</typeparam>
/// <typeparam name="TResponse">The type of each streamed item.</typeparam>
public sealed class StreamDiagnosticsBehavior<TRequest, TResponse> : IStreamPipelineBehavior<TRequest, TResponse>, IOrderBehavior
    where TRequest : IStreamRequest<TResponse>
{
    private static readonly string RequestName = typeof(TRequest).Name;

    /// <summary>Runs as one of the outermost stream behaviors so it measures the whole stream.</summary>
    public int Order => int.MinValue + 1;

    /// <inheritdoc />
    public async IAsyncEnumerable<TResponse> Handle(TRequest request, Func<IAsyncEnumerable<TResponse>> continuation, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(continuation);

        using var activity = MediarqDiagnostics.ActivitySource.StartActivity($"Mediarq:Stream {RequestName}", ActivityKind.Internal);
        activity?.SetTag("mediarq.request_type", typeof(TRequest).FullName);

        var startTimestamp = Stopwatch.GetTimestamp();
        long itemCount = 0;
        var succeeded = false;
        try
        {
            await foreach (var item in continuation().WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                itemCount++;
                yield return item;
            }

            succeeded = true;
        }
        finally
        {
            activity?.SetTag("mediarq.stream.item_count", itemCount);
            activity?.SetStatus(succeeded ? ActivityStatusCode.Ok : ActivityStatusCode.Error);
            MediarqDiagnostics.Record(RequestName, Stopwatch.GetElapsedTime(startTimestamp), succeeded);
        }
    }
}
