using System.Diagnostics.CodeAnalysis;

namespace Mediarq.Core.Common.Results;

public class Result
{
    public bool IsSuccess { get; init; }

    public bool IsFailure => !IsSuccess;
    
    public Error Error { get; init; }
    
    protected Result(bool isSuccess, Error error)
    {
        if(error is null)
            throw new ArgumentNullException(nameof(error), "A failure result must have an error."); 

        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("A successful result cannot have an error.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("A failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }
    
    public static Result Success() => new(true, Error.None);

    public static Result<T> Success<T>(T value) => new(value, true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<T> Failure<T>(Error error) => new(default, false, error);

    public static implicit operator Result(Error error) => Failure(error);
}

public class Result<TValue> : Result
{
    private readonly TValue _value;

    public Result(TValue value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("The value of a failure result can't be accessed");

    public static implicit operator Result<TValue>(TValue value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
    public static Result<TValue> ValidationFailure(Error error) => new(default, false, error);
}