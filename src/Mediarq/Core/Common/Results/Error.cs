namespace Mediarq.Core.Common.Results;

/// <summary>
/// Represents a standardized error used within the Mediarq framework to describe
/// failures, conflicts, or other exceptional conditions during request processing.
/// </summary>
/// <remarks>
/// The <see cref="ResultError"/> record provides a structured way to communicate errors throughout
/// the Mediarq pipeline, command handlers, and validation layers.  
/// 
/// Each <see cref="ResultError"/> instance contains a unique <see cref="Code"/> identifier,
/// a human-readable <see cref="Message"/>, and an <see cref="ErrorType"/> describing
/// the category or severity of the issue.  
/// 
/// Common error types include validation failures, not found errors, and business conflicts.
/// </remarks>
public record ResultError
{
    /// <summary>
    /// Represents a default empty error, typically used to indicate that no error occurred.
    /// </summary>
    public static readonly ResultError None = new(string.Empty, string.Empty, ErrorType.Failure);

    /// <summary>
    /// Represents an error indicating that a null or missing value was provided.
    /// </summary>
    public static readonly ResultError NullValue = new("General.Null", "Null value was provided", ErrorType.Failure);

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultError"/> record.
    /// </summary>
    /// <param name="code">A unique error code identifying the nature of the error.</param>
    /// <param name="message">A human-readable description of the error.</param>
    /// <param name="type">The category of the error, as defined by <see cref="ErrorType"/>.</param>
    public ResultError(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    /// <summary>
    /// Gets the unique identifier or code of the error.
    /// </summary>
    public string Code { get; init; }

    /// <summary>
    /// Gets the human-readable message that describes the error.
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// Gets the category of the error, such as Failure, NotFound, Conflict, or Problem.
    /// </summary>
    public ErrorType Type { get; init; }

    /// <summary>
    /// Creates a new <see cref="ResultError"/> representing a general failure.
    /// </summary>
    /// <param name="code">A unique error code.</param>
    /// <param name="message">A description of the failure.</param>
    /// <returns>An <see cref="ResultError"/> instance representing the failure.</returns>
    public static ResultError Failure(string code, string message) => new(code, message, ErrorType.Failure);

    /// <summary>
    /// Creates a new <see cref="ResultError"/> representing a "not found" condition.
    /// </summary>
    /// <param name="code">A unique error code.</param>
    /// <param name="message">A description of the missing resource or entity.</param>
    /// <returns>An <see cref="ResultError"/> instance representing the "not found" condition.</returns>
    public static ResultError NotFound(string code, string message) => new(code, message, ErrorType.NotFound);

    /// <summary>
    /// Creates a new <see cref="ResultError"/> representing a business or logical conflict.
    /// </summary>
    /// <param name="code">A unique error code.</param>
    /// <param name="message">A description of the conflicting condition.</param>
    /// <returns>An <see cref="ResultError"/> instance representing the conflict.</returns>
    public static ResultError Conflict(string code, string message) => new(code, message, ErrorType.Conflict);

    /// <summary>
    /// Creates a new <see cref="ResultError"/> representing a general problem or unexpected issue.
    /// </summary>
    /// <param name="code">A unique error code.</param>
    /// <param name="message">A description of the problem.</param>
    /// <returns>An <see cref="ResultError"/> instance representing the problem.</returns>
    public static ResultError Problem(string code, string message) => new(code, message, ErrorType.Problem);
}
