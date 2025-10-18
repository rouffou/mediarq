namespace Mediarq.Core.Common.Results;

public sealed record ValidationError : Error
{
    public ValidationError(Error[] errors) 
        : base(
            "Validation.General",
            "One or More validation errors occurred",
            ErrorType.Validation)
    {
        if(errors is null)
            throw new ArgumentNullException(nameof(errors));

        Errors = errors;
    }

    public Error[] Errors { get; init; }

    public static ValidationError FromResults(IEnumerable<Result> results)
        => new(results.Where(r => r.IsFailure).Select(r => r.Error).ToArray());
}
