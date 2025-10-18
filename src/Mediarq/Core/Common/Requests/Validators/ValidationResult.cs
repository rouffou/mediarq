namespace Mediarq.Core.Common.Requests.Validators;

public record ValidationResult
{
    public bool IsValid => !Errors.Any();

    public List<ValidationPropertyError> Errors { get; } = new();

    public ValidationResult()
    {
    }

    public ValidationResult(IEnumerable<ValidationPropertyError> errors)
    {
        if (errors is null)
            throw new ArgumentNullException(nameof(errors));

        Errors.AddRange(errors);
    }

    public static ValidationResult Success() => new();

    public static ValidationResult Failure(IEnumerable<ValidationPropertyError> errors) => new(errors);
}