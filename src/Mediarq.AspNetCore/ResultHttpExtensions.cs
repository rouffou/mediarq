using Mediarq.Core.Common.Results;
using Microsoft.AspNetCore.Http;

namespace Mediarq.AspNetCore;

/// <summary>
/// Maps Mediarq <see cref="Result"/> / <see cref="Result{T}"/> values to ASP.NET Core
/// <see cref="IResult"/> responses — HTTP status codes and RFC 7807 ProblemDetails.
/// </summary>
public static class ResultHttpExtensions
{
    /// <summary>
    /// Maps an <see cref="ErrorType"/> to its corresponding HTTP status code.
    /// </summary>
    /// <param name="errorType">The error category.</param>
    /// <returns>The HTTP status code (e.g. 400 for validation, 404 for not found).</returns>
    public static int ToStatusCode(this ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError,
    };

    /// <summary>
    /// Converts a <see cref="Result"/> to an <see cref="IResult"/>: <c>204 No Content</c> on success,
    /// or a problem / validation-problem response on failure.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <returns>The corresponding <see cref="IResult"/>.</returns>
    public static IResult ToHttpResult(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsSuccess ? TypedResults.NoContent() : ToProblem(result.Error);
    }

    /// <summary>
    /// Converts a <see cref="Result{T}"/> to an <see cref="IResult"/>: <c>200 OK</c> with the value on
    /// success, or a problem / validation-problem response on failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <returns>The corresponding <see cref="IResult"/>.</returns>
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : ToProblem(result.Error);
    }

    /// <summary>
    /// Awaits the task and converts the <see cref="Result"/> to an <see cref="IResult"/>.
    /// </summary>
    /// <param name="resultTask">The task producing the result.</param>
    /// <returns>The corresponding <see cref="IResult"/>.</returns>
    public static async Task<IResult> ToHttpResultAsync(this Task<Result> resultTask)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        return (await resultTask.ConfigureAwait(false)).ToHttpResult();
    }

    /// <summary>
    /// Awaits the task and converts the <see cref="Result{T}"/> to an <see cref="IResult"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="resultTask">The task producing the result.</param>
    /// <returns>The corresponding <see cref="IResult"/>.</returns>
    public static async Task<IResult> ToHttpResultAsync<T>(this Task<Result<T>> resultTask)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        return (await resultTask.ConfigureAwait(false)).ToHttpResult();
    }

    private static IResult ToProblem(ResultError error)
    {
        if (error is ValidationError validationError)
        {
            var errors = validationError.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray());

            return TypedResults.ValidationProblem(errors);
        }

        return TypedResults.Problem(
            detail: error.Message,
            statusCode: error.Type.ToStatusCode(),
            title: string.IsNullOrEmpty(error.Code) ? null : error.Code);
    }
}
