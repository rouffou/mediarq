namespace Mediarq.Core.Common.Results;

public record Error
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new("General.Null", "Null value was provided", ErrorType.Failure);

    public Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public string Code { get; init; }

    public string Message { get; init; }

    public ErrorType Type { get; init; }

    public static Error Failure(string code, string message) => new(code, message, ErrorType.Failure);

    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);

    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);

    public static Error Problem(string code, string message) => new(code, message, ErrorType.Problem);
}
