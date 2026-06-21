namespace Mediarq.Core.Common.Exceptions;

/// <summary>
/// Thrown when handling a request annotated with <c>ITimeoutRequest</c> exceeds its configured timeout.
/// </summary>
public sealed class RequestTimeoutException : MediarqException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestTimeoutException"/> class.
    /// </summary>
    /// <param name="requestType">The request type that timed out.</param>
    /// <param name="timeout">The timeout that was exceeded.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="requestType"/> is <see langword="null"/>.</exception>
    public RequestTimeoutException(Type requestType, TimeSpan timeout)
        : base($"Request '{(requestType ?? throw new ArgumentNullException(nameof(requestType))).Name}' exceeded its timeout of {timeout}.")
    {
        RequestType = requestType;
        Timeout = timeout;
    }

    /// <summary>Gets the request type that timed out.</summary>
    public Type RequestType { get; }

    /// <summary>Gets the timeout that was exceeded.</summary>
    public TimeSpan Timeout { get; }
}
