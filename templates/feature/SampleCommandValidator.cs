using System.Collections.Generic;
using Mediarq.Core.Common.Requests.Validators;

namespace Mediarq.App.Features;

/// <summary>Validates <see cref="SampleCommand"/> before the handler runs.</summary>
public sealed class SampleCommandValidator : IValidator<SampleCommand>
{
    public IEnumerable<ValidationResult> Validate(SampleCommand instance)
    {
        if (string.IsNullOrWhiteSpace(instance.Name))
        {
            yield return ValidationResult.Failure([new ValidationPropertyError(nameof(instance.Name), "Name is required.")]);
        }
        else
        {
            yield return ValidationResult.Success();
        }
    }
}
