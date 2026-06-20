namespace Mediarq.Core.Common.Results;

/// <summary>
/// Represents a composite validation error that aggregates multiple individual <see cref="ResultError"/> instances.
/// </summary>
/// <remarks>
/// The <see cref="ValidationError"/> record type is used to encapsulate one or more validation failures 
/// that occurred during command or query validation.  
/// 
/// It inherits from <see cref="ResultError"/> and provides additional context by grouping 
/// all validation-related errors under a single parent instance.
/// </remarks>
public sealed record ValidationError : ResultError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> record with the specified validation errors.
    /// </summary>
    /// <param name="errors">An array of <see cref="ResultError"/> representing the individual validation errors.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is <see langword="null"/>.</exception>
    public ValidationError(ResultError[] errors) 
        : base(
            "Validation.General",
            "One or More validation errors occurred",
            ErrorType.Validation)
    {
        ArgumentNullException.ThrowIfNull(errors);

        Errors = errors;
    }

    /// <summary>
    /// Gets the collection of individual validation errors that compose this validation result.
    /// </summary>
    public ResultError[] Errors { get; init; }

    /// <summary>
    /// Creates a new <see cref="ValidationError"/> instance by extracting all errors from a collection of <see cref="Result"/> objects.
    /// </summary>
    /// <param name="results">The results to aggregate validation errors from.</param>
    /// <returns>
    /// A <see cref="ValidationError"/> containing all <see cref="ResultError"/> instances 
    /// found in the failed results of the provided <paramref name="results"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// var results = new[]
    /// {
    ///     Result.Failure(new ResultError("Name.Required", "Name is required.", ErrorType.Validation)),
    ///     Result.Success()
    /// };
    ///
    /// var validationError = ValidationError.FromResults(results);
    /// </code>
    /// </example>
    public static ValidationError FromResults(IEnumerable<Result> results)
        => new([.. results.Where(r => r.IsFailure).Select(r => r.Error)]);
}
