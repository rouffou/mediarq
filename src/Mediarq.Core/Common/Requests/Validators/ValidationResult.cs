namespace Mediarq.Core.Common.Requests.Validators;

/// <summary>
/// Represents the result of a validation process performed on a command, query, or request.
/// </summary>
/// <remarks>
/// The <see cref="ValidationResult"/> record encapsulates the outcome of validation logic
/// executed by implementations of <see cref="IValidator{T}"/>.  
/// 
/// It provides a collection of <see cref="ValidationPropertyError"/> instances describing
/// individual property-level validation failures, and exposes the <see cref="IsValid"/> property
/// to quickly determine whether the validation succeeded.
/// </remarks>
/// <example>
/// <code>
/// var result = new ValidationResult(new[]
/// {
///     new ValidationPropertyError("Email", "Email is required."),
///     new ValidationPropertyError("Password", "Password must contain at least 8 characters.")
/// });
///
/// if (!result.IsValid)
/// {
///     foreach (var error in result.Errors)
///         Console.WriteLine(error);
/// }
/// </code>
/// </example>
public record ValidationResult
{

    /// <summary>
    /// Gets a value indicating whether the validation passed successfully.
    /// </summary>
    /// <value><c>true</c> if no validation errors were found; otherwise, <c>false</c>.</value>
    public bool IsValid => Errors.Count == 0;

    /// <summary>
    /// Gets the collection of property-level validation errors.
    /// </summary>
    /// <value>
    /// A list of <see cref="ValidationPropertyError"/> objects representing each property that failed validation.
    /// </value>
    public List<ValidationPropertyError> Errors { get; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult"/> class with no errors.
    /// </summary>
    public ValidationResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult"/> class with a predefined set of errors.
    /// </summary>
    /// <param name="errors">The collection of validation errors that occurred.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is <c>null</c>.</exception>
    public ValidationResult(IEnumerable<ValidationPropertyError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        Errors.AddRange(errors);
    }

    /// <summary>
    /// Creates a successful <see cref="ValidationResult"/> with no validation errors.
    /// </summary>
    /// <returns>A <see cref="ValidationResult"/> instance representing a successful validation.</returns>
    public static ValidationResult Success() => new();

    /// <summary>
    /// Creates a failed <see cref="ValidationResult"/> with the specified validation errors.
    /// </summary>
    /// <param name="errors">The collection of validation errors that occurred.</param>
    /// <returns>A <see cref="ValidationResult"/> instance representing a failed validation.</returns>
    public static ValidationResult Failure(IEnumerable<ValidationPropertyError> errors) => new(errors);
}
