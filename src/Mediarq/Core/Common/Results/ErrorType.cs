namespace Mediarq.Core.Common.Results;

/// <summary>
/// Defines the classification of errors that can occur within the Mediarq framework.
/// </summary>
/// <remarks>
/// The <see cref="ErrorType"/> enumeration categorizes different kinds of application-level errors
/// that may occur during request handling, validation, or command/query execution.  
/// 
/// It helps standardize error management and enables consistent error responses across
/// the Mediarq pipeline.
/// </remarks>
public enum ErrorType
{
    /// <summary>
    /// Represents a general or unspecified failure.
    /// Typically used when an operation fails without a more specific error category.
    /// </summary>
    Failure = 0,

    /// <summary>
    /// Represents a validation error, usually caused by invalid input data.
    /// This type is often returned by validators or business rule checks.
    /// </summary>
    Validation = 1,

    /// <summary>
    /// Represents a generic problem, such as an unexpected system or infrastructure error.
    /// </summary>
    Problem = 2,

    /// <summary>
    /// Indicates that a requested resource or entity was not found.
    /// Commonly used for queries or lookups that return no results.
    /// </summary>
    NotFound = 3,

    /// <summary>
    /// Represents a logical or business conflict.
    /// Typically used when an operation cannot proceed due to a conflicting state.
    /// </summary>
    Conflict = 4,

    /// <summary>
    /// Indicates that the current user or system actor is not authorized
    /// to perform the requested operation.
    /// </summary>
    Unauthorized = 5,
}
