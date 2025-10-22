namespace Mediarq.Core.Common.Requests.Validators;

public record ValidationResult
{
    public bool IsValid => Errors.Count == 0;

    public List<ValidationPropertyError> Errors { get; } = [];

    public ValidationResult()
    {
    }

    public ValidationResult(IEnumerable<ValidationPropertyError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        Errors.AddRange(errors);
    }

    public static ValidationResult Success() => new();

    public static ValidationResult Failure(IEnumerable<ValidationPropertyError> errors) => new(errors);
}
