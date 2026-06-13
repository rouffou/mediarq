namespace Mediarq.Core.Common.Exceptions;

/// <summary>
/// Base type for all exceptions raised by the Mediarq framework.
/// </summary>
public class MediarqException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediarqException"/> class with the specified message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MediarqException(string message) : base(message)
    {
    }
}
