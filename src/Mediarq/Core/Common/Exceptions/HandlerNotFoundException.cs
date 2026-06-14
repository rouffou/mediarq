namespace Mediarq.Core.Common.Exceptions;

/// <summary>
/// The exception thrown when no handler can be resolved for a dispatched request.
/// </summary>
public class HandlerNotFoundException : MediarqException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerNotFoundException"/> class for the given request type.
    /// </summary>
    /// <param name="requestType">The request type for which no handler was found.</param>
    public HandlerNotFoundException(Type requestType)
        : base($"No handler found for request type '{requestType.FullName}'")
    {
    }
}
