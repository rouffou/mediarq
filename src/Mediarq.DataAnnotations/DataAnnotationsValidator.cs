using Mediarq.Core.Common.Requests.Validators;
using DaContext = System.ComponentModel.DataAnnotations.ValidationContext;
using DaResult = System.ComponentModel.DataAnnotations.ValidationResult;
using DaValidator = System.ComponentModel.DataAnnotations.Validator;

namespace Mediarq.DataAnnotations;

/// <summary>
/// An <see cref="IValidator{T}"/> that validates a request using
/// <c>System.ComponentModel.DataAnnotations</c> attributes (e.g. <c>[Required]</c>, <c>[Range]</c>).
/// </summary>
/// <typeparam name="T">The request type to validate.</typeparam>
/// <remarks>
/// Types without DataAnnotations attributes simply pass. This adapter uses reflection
/// (<c>Validator.TryValidateObject</c>) and is therefore not trimming/AOT friendly.
/// </remarks>
public sealed class DataAnnotationsValidator<T> : IValidator<T>
{
    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(T instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        var context = new DaContext(instance);
        var results = new List<DaResult>();

        if (DaValidator.TryValidateObject(instance, context, results, validateAllProperties: true))
        {
            return [ValidationResult.Success()];
        }

        var errors = results
            .SelectMany(result => (result.MemberNames.Any() ? result.MemberNames : [string.Empty])
                .Select(member => new ValidationPropertyError(member, result.ErrorMessage ?? "Invalid value.")))
            .ToArray();

        return [ValidationResult.Failure(errors)];
    }
}
