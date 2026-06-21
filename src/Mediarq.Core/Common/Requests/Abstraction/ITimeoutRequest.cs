namespace Mediarq.Core.Common.Requests.Abstraction;

/// <summary>
/// Marks a request that should not run longer than <see cref="Timeout"/>. When the
/// <c>TimeoutBehavior</c> is registered (via <c>AddMediarqTimeout()</c>), a request implementing this
/// interface whose handling exceeds the timeout fails with a <c>RequestTimeoutException</c>.
/// </summary>
/// <remarks>
/// The timeout is <em>pessimistic</em>: it frees the caller once the deadline passes, but the handler is
/// not forcibly aborted (the runtime cannot abort arbitrary code). For true cooperative cancellation,
/// handlers should also observe the <see cref="CancellationToken"/> they are given.
/// </remarks>
public interface ITimeoutRequest
{
    /// <summary>Gets the maximum time the request is allowed to take. A non-positive value disables the timeout.</summary>
    TimeSpan Timeout { get; }
}
