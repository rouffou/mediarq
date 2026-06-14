namespace Mediarq.Core.Common.Results;

/// <summary>
/// Functional (railway-oriented) combinators over <see cref="Result"/> and <see cref="Result{T}"/>:
/// transform, chain, branch and inspect results without manual <c>IsSuccess</c> checks.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Transforms the value of a successful result; propagates the error otherwise.
    /// </summary>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map)
    {
        ArgumentNullException.ThrowIfNull(map);
        return result.IsSuccess ? Result.Success(map(result.Value)) : Result.Failure<TOut>(result.Error);
    }

    /// <summary>
    /// Chains another result-producing operation onto a successful result; propagates the error otherwise.
    /// </summary>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> bind)
    {
        ArgumentNullException.ThrowIfNull(bind);
        return result.IsSuccess ? bind(result.Value) : Result.Failure<TOut>(result.Error);
    }

    /// <summary>
    /// Returns <paramref name="onSuccess"/> applied to the value, or <paramref name="onFailure"/> applied to the error.
    /// </summary>
    public static TOut Match<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> onSuccess, Func<ResultError, TOut> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
    }

    /// <summary>
    /// Returns <paramref name="onSuccess"/> or <paramref name="onFailure"/> depending on the result state.
    /// </summary>
    public static TOut Match<TOut>(this Result result, Func<TOut> onSuccess, Func<ResultError, TOut> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    }

    /// <summary>
    /// Runs a side effect on the value when the result is successful, then returns the same result.
    /// </summary>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (result.IsSuccess)
        {
            action(result.Value);
        }
        return result;
    }

    /// <summary>
    /// Turns a successful result into a failure with <paramref name="error"/> when <paramref name="predicate"/> is not met.
    /// </summary>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, ResultError error)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(error);
        return result.IsSuccess && !predicate(result.Value) ? Result.Failure<T>(error) : result;
    }

    /// <summary>
    /// Asynchronous <see cref="Map{TIn, TOut}(Result{TIn}, Func{TIn, TOut})"/> over a <see cref="Task{TResult}"/> of result.
    /// </summary>
    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, TOut> map)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        return (await resultTask.ConfigureAwait(false)).Map(map);
    }

    /// <summary>
    /// Asynchronous <see cref="Bind{TIn, TOut}(Result{TIn}, Func{TIn, Result{TOut}})"/> with an async continuation.
    /// </summary>
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result<TOut>>> bind)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(bind);

        var result = await resultTask.ConfigureAwait(false);
        return result.IsSuccess ? await bind(result.Value).ConfigureAwait(false) : Result.Failure<TOut>(result.Error);
    }

    /// <summary>
    /// Asynchronous <see cref="Match{TIn, TOut}(Result{TIn}, Func{TIn, TOut}, Func{ResultError, TOut})"/>.
    /// </summary>
    public static async Task<TOut> MatchAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, TOut> onSuccess, Func<ResultError, TOut> onFailure)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        return (await resultTask.ConfigureAwait(false)).Match(onSuccess, onFailure);
    }

    /// <summary>
    /// Asynchronous <see cref="Tap{T}(Result{T}, Action{T})"/>.
    /// </summary>
    public static async Task<Result<T>> TapAsync<T>(this Task<Result<T>> resultTask, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        return (await resultTask.ConfigureAwait(false)).Tap(action);
    }
}
