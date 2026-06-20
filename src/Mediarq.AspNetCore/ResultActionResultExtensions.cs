using Mediarq.Core.Common.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mediarq.AspNetCore;

/// <summary>
/// Maps Mediarq <see cref="Result"/> / <see cref="Result{T}"/> values to MVC
/// <see cref="IActionResult"/> responses (for classic controllers), mirroring
/// <see cref="ResultHttpExtensions"/> for minimal APIs.
/// </summary>
public static class ResultActionResultExtensions
{
    /// <summary>
    /// Converts a <see cref="Result"/> to an <see cref="IActionResult"/>: <c>204 No Content</c> on
    /// success, or a problem / validation-problem response on failure.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <param name="configureProblem">Optional hook to customize the <see cref="ProblemDetails"/> on failure.</param>
    /// <returns>The corresponding <see cref="IActionResult"/>.</returns>
    public static IActionResult ToActionResult(this Result result, Action<ProblemDetails>? configureProblem = null)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsSuccess ? new NoContentResult() : ToProblem(result.Error, configureProblem);
    }

    /// <summary>
    /// Converts a <see cref="Result{T}"/> to an <see cref="IActionResult"/>: <c>200 OK</c> with the
    /// value on success, or a problem / validation-problem response on failure.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="configureProblem">Optional hook to customize the <see cref="ProblemDetails"/> on failure.</param>
    /// <returns>The corresponding <see cref="IActionResult"/>.</returns>
    public static IActionResult ToActionResult<T>(this Result<T> result, Action<ProblemDetails>? configureProblem = null)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsSuccess ? new OkObjectResult(result.Value) : ToProblem(result.Error, configureProblem);
    }

    /// <summary>Awaits the task and converts the <see cref="Result"/> to an <see cref="IActionResult"/>.</summary>
    public static async Task<IActionResult> ToActionResultAsync(this Task<Result> resultTask, Action<ProblemDetails>? configureProblem = null)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        return (await resultTask.ConfigureAwait(false)).ToActionResult(configureProblem);
    }

    /// <summary>Awaits the task and converts the <see cref="Result{T}"/> to an <see cref="IActionResult"/>.</summary>
    public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Result<T>> resultTask, Action<ProblemDetails>? configureProblem = null)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        return (await resultTask.ConfigureAwait(false)).ToActionResult(configureProblem);
    }

    private static IActionResult ToProblem(ResultError error, Action<ProblemDetails>? configureProblem)
    {
        if (error is ValidationError validationError)
        {
            var errors = validationError.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray());

            var validationProblem = new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
            };

            configureProblem?.Invoke(validationProblem);
            return new ObjectResult(validationProblem) { StatusCode = validationProblem.Status };
        }

        var problem = new ProblemDetails
        {
            Detail = error.Message,
            Status = error.Type.ToStatusCode(),
            Title = string.IsNullOrEmpty(error.Code) ? null : error.Code,
        };

        configureProblem?.Invoke(problem);
        return new ObjectResult(problem) { StatusCode = problem.Status };
    }
}
