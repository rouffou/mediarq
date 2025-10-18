namespace Mediarq.Core.Common.Requests.Validators;

public interface IValidator<in T>
{
    IEnumerable<ValidationResult> Validate(T instance);
}
