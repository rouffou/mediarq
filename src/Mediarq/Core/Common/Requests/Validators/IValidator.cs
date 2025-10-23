namespace Mediarq.Core.Common.Requests.Validators;

/// <summary>
/// Defines a contract for validating instances of a specified type.
/// </summary>
/// <remarks>
/// Implementations of <see cref="IValidator{T}"/> encapsulate validation logic for a given request,  
/// command, or query type within the Mediarq pipeline.  
/// 
/// Validators are automatically executed by the <c>ValidationBehavior&lt;TRequest, TResponse&gt;</c>  
/// before the associated request handler is invoked.  
/// 
/// Each validation rule returns one or more <see cref="ValidationResult"/> instances,  
/// which describe whether the validation succeeded and provide details about validation errors if any.
/// </remarks>
/// <typeparam name="T">
/// The type of object to validate. This is typically a command or query implementing <c>ICommandOrQuery&lt;TResponse&gt;</c>.
/// </typeparam>
/// <returns>
/// A collection of <see cref="ValidationResult"/> objects representing the outcome of each validation rule.  
/// If all rules succeed, this collection should be empty or contain only valid results.
/// </returns>
/// <example>
/// <code>
/// public class CreateUserCommandValidator : IValidator&lt;CreateUserCommand&gt;
/// {
///     public IEnumerable&lt;ValidationResult&gt; Validate(CreateUserCommand instance)
///     {
///         if (string.IsNullOrWhiteSpace(instance.Email))
///             yield return ValidationResult.Invalid(nameof(instance.Email), "Email is required.");
///
///         if (!instance.Email.Contains("@"))
///             yield return ValidationResult.Invalid(nameof(instance.Email), "Email must be valid.");
///
///         yield return ValidationResult.Valid(); // Default valid result
///     }
/// }
/// </code>
/// </example>
public interface IValidator<in T>
{
    /// <summary>
    /// Executes validation rules on the specified instance.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <returns>
    /// A collection of <see cref="ValidationResult"/> objects describing the validation outcomes.
    /// </returns>
    IEnumerable<ValidationResult> Validate(T instance);
}
