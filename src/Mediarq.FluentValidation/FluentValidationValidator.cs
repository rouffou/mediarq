using Mediarq.Core.Common.Requests.Validators;
using FV = global::FluentValidation;

namespace Mediarq.FluentValidation;

/// <summary>
/// Adapts one or more FluentValidation validators for <typeparamref name="T"/> to Mediarq's
/// <see cref="IValidator{T}"/>, so they run inside the Mediarq validation pipeline.
/// </summary>
/// <typeparam name="T">The validated request type.</typeparam>
public sealed class FluentValidationValidator<T> : IValidator<T>
{
    private readonly IEnumerable<FV.IValidator<T>> _validators;

    /// <summary>
    /// Initializes the adapter with the FluentValidation validators to bridge.
    /// </summary>
    /// <param name="validators">The FluentValidation validators for <typeparamref name="T"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="validators"/> is <see langword="null"/>.</exception>
    public FluentValidationValidator(IEnumerable<FV.IValidator<T>> validators)
    {
        ArgumentNullException.ThrowIfNull(validators);
        _validators = validators;
    }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(T instance)
        => ValidateAsync(instance).GetAwaiter().GetResult();

    /// <inheritdoc />
    public async Task<IEnumerable<ValidationResult>> ValidateAsync(T instance, CancellationToken cancellationToken = default)
    {
        var results = new List<ValidationResult>();

        foreach (var validator in _validators)
        {
            FV.Results.ValidationResult result = await validator.ValidateAsync(instance, cancellationToken).ConfigureAwait(false);
            if (!result.IsValid)
            {
                results.Add(new ValidationResult(
                    result.Errors.Select(e => new ValidationPropertyError(e.PropertyName, e.ErrorMessage))));
            }
        }

        return results;
    }
}
