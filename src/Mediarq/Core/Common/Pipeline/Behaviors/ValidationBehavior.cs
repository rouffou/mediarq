using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Results;
using System.Linq.Expressions;
using System.Reflection;

namespace Mediarq.Core.Common.Pipeline.Behaviors;

/// <summary>
/// Represents a pipeline behavior that performs validation on incoming requests
/// before they are passed to the next handler in the Mediarq pipeline.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request being validated.
/// Must implement <see cref="ICommandOrQuery{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned by the request handler.
/// </typeparam>
/// <remarks>
/// This behavior ensures that any registered validators for the given <typeparamref name="TRequest"/>
/// are executed before the request reaches its handler.  
/// If validation errors occur, the behavior short-circuits the pipeline and returns a failure result
/// instead of calling the next delegate.  
/// This is conceptually similar to FluentValidation integration in MediatR, but adapted for the Mediarq architecture.
/// </remarks>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    // Factory that converts a ValidationError into the failed TResponse. It is built once per
    // closed generic type (Result or Result&lt;T&gt;) and cached, avoiding per-request reflection.
    // It is null when TResponse is neither Result nor Result&lt;T&gt;.
    private static readonly Func<ValidationError, TResponse>? FailureFactory = BuildFailureFactory();

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">
    /// A collection of validators that apply validation rules to the incoming request.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="validators"/> is <see langword="null"/>.
    /// </exception>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Executes the validation logic for the incoming request and determines whether to continue
    /// with the next behavior or return a validation failure result.
    /// </summary>
    /// <param name="context">
    /// The current request context containing the request and metadata.
    /// </param>
    /// <param name="handle">
    /// The delegate representing the next step in the pipeline or the final handler.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to propagate cancellation signals.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation that produces the response (<typeparamref name="TResponse"/>).  
    /// If validation fails, a failed <see cref="Result"/> or <see cref="Result{T}"/> is returned instead.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when validation fails but <typeparamref name="TResponse"/> is not a supported <see cref="Result"/> type.
    /// </exception>
    /// <remarks>
    /// The behavior performs the following steps:
    /// <list type="number">
    ///   <item><description>Executes all registered <see cref="IValidator{TRequest}"/> instances against the request.</description></item>
    ///   <item><description>Collects all validation errors from the validators.</description></item>
    ///   <item><description>If no validation errors are found, continues to the next pipeline behavior.</description></item>
    ///   <item><description>If validation errors exist:
    ///       <list type="bullet">
    ///           <item><description>Creates a <see cref="ValidationError"/> object containing all property-level errors.</description></item>
    ///           <item><description>Returns a failed <see cref="Result"/> or <see cref="Result{T}"/> instance based on <typeparamref name="TResponse"/>.</description></item>
    ///       </list>
    ///   </description></item>
    /// </list>
    /// Supported response types:
    /// <list type="bullet">
    ///   <item><description><see cref="Result"/></description></item>
    ///   <item><description><see cref="Result{T}"/></description></item>
    /// </list>
    /// </remarks>
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        var failures = new List<ValidationPropertyError>();
        foreach (var validator in _validators)
        {
            var results = await validator.ValidateAsync(context.Request, cancellationToken).ConfigureAwait(false);
            failures.AddRange(results.Where(r => !r.IsValid).SelectMany(r => r.Errors));
        }

        // No error → continue the pipeline.
        if (failures.Count == 0)
        {
            return await handle().ConfigureAwait(false);
        }

        // Aggregate every individual property error.
        ResultError[] propertyErrors = [.. failures
            .Select(e => new ResultError(
                $"Validation.{typeof(TRequest).Name}.{e.PropertyName}",
                e.ErrorMessage,
                ErrorType.Validation))];

        var validationError = new ValidationError(propertyErrors);

        // TResponse is neither Result nor Result<T>: a validation failure cannot be expressed.
        if (FailureFactory is null)
        {
            throw new InvalidOperationException($"Validation failed but TResponse type '{typeof(TResponse).Name}' is not a supported Result type.");
        }

        return FailureFactory(validationError);
    }

    /// <summary>
    /// Builds, once per closed generic type, the delegate that turns a <see cref="ValidationError"/>
    /// into a failed <typeparamref name="TResponse"/>. Returns <see langword="null"/> when
    /// <typeparamref name="TResponse"/> is neither <see cref="Result"/> nor <see cref="Result{T}"/>.
    /// </summary>
    private static Func<ValidationError, TResponse>? BuildFailureFactory()
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return static error => (TResponse)(object)Result.Failure(error);
        }

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type valueType = typeof(TResponse).GetGenericArguments()[0];
            MethodInfo method = typeof(Result<>)
                .MakeGenericType(valueType)
                .GetMethod(nameof(Result<object>.ValidationFailure))!;

            ParameterExpression errorParam = Expression.Parameter(typeof(ValidationError), "error");
            Expression body = Expression.Convert(Expression.Call(method, errorParam), typeof(TResponse));

            return Expression.Lambda<Func<ValidationError, TResponse>>(body, errorParam).Compile();
        }

        return null;
    }
}
