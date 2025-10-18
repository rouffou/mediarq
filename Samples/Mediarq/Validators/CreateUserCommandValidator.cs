using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Samples.Commands;

namespace Mediarq.Samples.Validators;

public class CreateUserCommandValidator : IValidator<CreateUserCommand>
{
    public IEnumerable<ValidationResult> Validate(CreateUserCommand instance)
    {
        var errors = new List<ValidationPropertyError>();

        if (instance is null)
        {
            yield return ValidationResult.Failure(new List<ValidationPropertyError> { new ValidationPropertyError(string.Empty, "Request cannot be null") });
            yield break;
        }

        if (string.IsNullOrWhiteSpace(instance.Name))
            errors.Add(new ValidationPropertyError(nameof(instance.Name), "Name cannot be empty."));

        // Exemple d'autre règle
        if (instance.Name != null && instance.Name.Length > 100)
            errors.Add(new ValidationPropertyError(nameof(instance.Name), "Name too long (max 100 chars)."));

        if (errors.Count > 0)
            yield return new ValidationResult(errors);
        else
            yield return ValidationResult.Success();
    }
}
