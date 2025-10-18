namespace Mediarq.Core.Common.Requests.Validators;

public record ValidationPropertyError
{
    public ValidationPropertyError(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }

    public string PropertyName { get; init; }

    public string ErrorMessage { get; init; }

    public override string ToString()
        => $"{PropertyName}: {ErrorMessage}";
}
