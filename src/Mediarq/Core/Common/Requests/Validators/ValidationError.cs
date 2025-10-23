namespace Mediarq.Core.Common.Requests.Validators;


/// <summary>
/// Represents a detailed validation error for a specific property within a request.
/// </summary>
/// <remarks>
/// This record is typically produced by validators implementing <see cref="IValidator{T}"/>  
/// when a property of a command or query fails validation.  
/// 
/// It provides both the name of the invalid property and the associated error message,  
/// which can be used by higher-level constructs like <see cref="ValidationError"/>  
/// or <see cref="Result"/> objects to report validation failures in a consistent way.
/// </remarks>
/// <example>
/// <code>
/// var error = new ValidationPropertyError("Email", "Email address is required.");
/// Console.WriteLine(error); 
/// // Output: "Email: Email address is required."
/// </code>
/// </example>
public record ValidationPropertyError
{

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationPropertyError"/> record.
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation.</param>
    /// <param name="errorMessage">The validation error message associated with the property.</param>
    public ValidationPropertyError(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Gets the name of the property that caused the validation failure.
    /// </summary>
    public string PropertyName { get; init; }

    /// <summary>
    /// Gets the human-readable validation error message.
    /// </summary>
    public string ErrorMessage { get; init; }

    /// <summary>
    /// Returns a string representation of the validation error, combining the property name and the error message.
    /// </summary>
    /// <returns>A formatted string in the form "PropertyName: ErrorMessage".</returns>
    public override string ToString()
        => $"{PropertyName}: {ErrorMessage}";
}
