using System.Diagnostics.CodeAnalysis;

namespace Mediarq.Core.Common.Results;

/// <summary>
/// Represents the outcome of an operation that can either succeed or fail.
/// </summary>
/// <remarks>
/// The <see cref="Result"/> class provides a consistent pattern for expressing success or failure
/// in application workflows without relying on exceptions for control flow.
/// 
/// It encapsulates an <see cref="IsSuccess"/> flag and a corresponding <see cref="ResultError"/> 
/// to describe failure reasons when applicable.  
/// 
/// Use this type for commands or operations that do not return a specific value.
/// For operations producing a value, use the generic version <see cref="Result{T}"/>.
/// </remarks>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the associated <see cref="ResultError"/> describing the error when the operation fails.
    /// If the result represents success, this value is <see cref="ResultError.None"/>.
    /// </summary>
    public ResultError Error { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation succeeded.</param>
    /// <param name="error">The error object associated with the result.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="error"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the combination of <paramref name="isSuccess"/> and <paramref name="error"/> is inconsistent:
    /// <list type="bullet">
    /// <item><description>A successful result cannot have a non-<see cref="ResultError.None"/> error.</description></item>
    /// <item><description>A failure result must have a valid error.</description></item>
    /// </list>
    /// </exception>
    protected Result(bool isSuccess, ResultError error)
    {
        if(error is null) {
            throw new ArgumentNullException(nameof(error), "A failure result must have an error.");
        }

        if (isSuccess && error != ResultError.None) {
            throw new InvalidOperationException("A successful result cannot have an error.");
        }

        if (!isSuccess && error == ResultError.None) {
            throw new InvalidOperationException("A failure result must have an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful <see cref="Result"/> instance with no associated value.
    /// </summary>
    public static Result Success() => new(true, ResultError.None);

    /// <summary>
    /// Creates a successful <see cref="Result{T}"/> instance containing a specified value.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="value">The value to be returned in the result.</param>
    public static Result<T> Success<T>(T value) => new(value, true, ResultError.None);

    /// <summary>
    /// Creates a failed <see cref="Result"/> instance with a specified error.
    /// </summary>
    /// <param name="error">The error describing the reason for failure.</param>
    public static Result Failure(ResultError error) => new(false, error);

    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> instance with a specified error.
    /// </summary>
    /// <typeparam name="T">The type of the expected result value.</typeparam>
    /// <param name="error">The error describing the reason for failure.</param>
    public static Result<T> Failure<T>(ResultError error) => new(default, false, error);

    /// <summary>
    /// Implicitly converts a <see cref="ResultError"/> into a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result(ResultError error) {
        return Failure(error);
    }
}

/// <summary>
/// Represents the outcome of an operation that produces a value of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">The type of the value returned when the operation succeeds.</typeparam>
/// <remarks>
/// The <see cref="Result{TValue}"/> class extends the non-generic <see cref="Result"/> type 
/// by encapsulating both the success or failure state of an operation and a resulting value.  
/// 
/// It provides implicit conversions from <typeparamref name="TValue"/> and <see cref="ResultError"/> 
/// to simplify usage in functional or command/query pipelines.
/// </remarks>
public class Result<TValue> : Result
{
    private readonly TValue _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    /// <param name="value">The value associated with the result, if successful.</param>
    /// <param name="isSuccess">Indicates whether the operation succeeded.</param>
    /// <param name="error">The error associated with the operation, if it failed.</param>
    public Result(TValue value, bool isSuccess, ResultError error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value of the result when the operation succeeds.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to access <see cref="Value"/> on a failed result.
    /// </exception>
    [NotNull]
    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("The value of a failure result can't be accessed");

    /// <summary>
    /// Implicitly converts a <typeparamref name="TValue"/> into a successful <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="value">The value to wrap inside a <see cref="Result{TValue}"/>.</param>
    /// <returns>
    /// A successful result containing the given <paramref name="value"/>, 
    /// or a failure result with <see cref="ResultError.NullValue"/> if <paramref name="value"/> is null.
    /// </returns>
    public static implicit operator Result<TValue>(TValue value) {
        return value is not null ? Success(value) : Failure<TValue>(ResultError.NullValue);
    }

    /// <summary>
    /// Implicitly converts a <see cref="ResultError"/> into a failed <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="error">The error to wrap into a failed result.</param>
    public static implicit operator Result<TValue>(ResultError error) {
        return Failure<TValue>(error);
    }

    /// <summary>
    /// Creates a validation failure result for the given <paramref name="error"/>.
    /// </summary>
    /// <param name="error">The validation error associated with the failure.</param>
    /// <returns>
    /// A failed <see cref="Result{TValue}"/> representing a validation error.
    /// </returns>
    public static Result<TValue> ValidationFailure(ResultError error) => new(default, false, error);
}
