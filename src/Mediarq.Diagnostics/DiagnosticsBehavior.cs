using System.Diagnostics;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Diagnostics;

/// <summary>
/// Pipeline behavior that records an <see cref="Activity"/> (distributed trace span) and metrics
/// (count + duration) for every dispatched request.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
public sealed class DiagnosticsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, IOrderBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    private static readonly string RequestName = typeof(TRequest).Name;

    /// <summary>
    /// Runs as one of the outermost behaviors so it measures the whole pipeline, just inside the
    /// exception processor (<see cref="int.MinValue"/>).
    /// </summary>
    public int Order => int.MinValue + 1;

    /// <inheritdoc />
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        using var activity = MediarqDiagnostics.ActivitySource.StartActivity($"Mediarq:{RequestName}", ActivityKind.Internal);
        if (activity is not null)
        {
            activity.SetTag("mediarq.request_type", typeof(TRequest).FullName);
            activity.SetTag("mediarq.request_id", context.RequestId);
            activity.SetTag("mediarq.correlation_id", context.CorrelationId);
        }

        var startTimestamp = Stopwatch.GetTimestamp();
        try
        {
            var response = await handle().ConfigureAwait(false);
            MediarqDiagnostics.Record(RequestName, Stopwatch.GetElapsedTime(startTimestamp), succeeded: true);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return response;
        }
        catch (Exception exception)
        {
            MediarqDiagnostics.Record(RequestName, Stopwatch.GetElapsedTime(startTimestamp), succeeded: false);
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            throw;
        }
    }
}
