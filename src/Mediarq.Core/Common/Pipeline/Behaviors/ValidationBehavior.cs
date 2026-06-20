using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Results;
using System.Diagnostics.CodeAnalysis;
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
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, IConditionalPipelineBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    private const string DynamicFallbackJustification =
        "The dynamic Result<T> failure factory is only built when no source-generated factory is " +
        "registered (the reflection-based AddMediarq assembly scan), which is not used on Native AOT. " +
        "The AOT path uses the generated AddMediarqHandlers(), which registers the factory via ValidationFailureRegistry.";

    private readonly IValidator<TRequest>[] _validators;
    private readonly IValidationMessageResolver? _messageResolver;

    // Converts a ValidationError into a failed TResponse, resolved without reflection or dynamic code:
    // for Result it is a static lambda; for Result<T> it comes from the source-generated
    // ValidationFailureRegistry. Null when TResponse is neither Result nor a registered Result<T>.
    private static readonly Func<ValidationError, TResponse>? FailureFactory = BuildSafeFailureFactory();

    // Lazily-built dynamic fallback for Result<T> when no generated factory was registered (scan mode).
    private static Func<ValidationError, TResponse>? _dynamicFailureFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">
    /// A collection of validators that apply validation rules to the incoming request.
    /// </param>
    /// <param name="messageResolver">
    /// Optional resolver used to localize/translate validation messages. When <see langword="null"/>,
    /// messages are used as-is.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="validators"/> is <see langword="null"/>.
    /// </exception>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, IValidationMessageResolver? messageResolver = null)
    {
        _validators = validators as IValidator<TRequest>[] ?? [.. validators];
        _messageResolver = messageResolver;
    }

    /// <summary>Active only when at least one validator is registered for this request type.</summary>
    public bool IsActive => _validators.Length > 0;

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
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = DynamicFallbackJustification)]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = DynamicFallbackJustification)]
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        // The failures list is only allocated once a validator actually reports an error, so the common
        // case (no validators, or all valid) stays allocation-free on this hot path.
        List<ValidationPropertyError>? failures = null;
        foreach (var validator in _validators)
        {
            var results = await validator.ValidateAsync(context.Request, cancellationToken).ConfigureAwait(false);
            foreach (var result in results)
            {
                if (!result.IsValid)
                {
                    failures ??= [];
                    failures.AddRange(result.Errors);
                }
            }
        }

        // No error → continue the pipeline.
        if (failures is null || failures.Count == 0)
        {
            return await handle().ConfigureAwait(false);
        }

        // Aggregate every individual property error, resolving (e.g. localizing) the message when a
        // resolver is registered.
        ResultError[] propertyErrors = [.. failures
            .Select(e => new ResultError(
                $"Validation.{typeof(TRequest).Name}.{e.PropertyName}",
                _messageResolver is null ? e.ErrorMessage : _messageResolver.Resolve(e.PropertyName, e.ErrorMessage),
                ErrorType.Validation))];

        var validationError = new ValidationError(propertyErrors);

        var factory = FailureFactory ?? ResolveDynamicFailureFactory();

        // TResponse is neither Result nor Result<T>: a validation failure cannot be expressed.
        if (factory is null)
        {
            throw new InvalidOperationException($"Validation failed but TResponse type '{typeof(TResponse).Name}' is not a supported Result type.");
        }

        return factory(validationError);
    }

    /// <summary>
    /// Builds the reflection-free failure factory: a static lambda for <see cref="Result"/>, or the
    /// factory registered by the source generator for a <see cref="Result{T}"/> response. Returns
    /// <see langword="null"/> when neither applies (for example the scan path for <see cref="Result{T}"/>).
    /// </summary>
    private static Func<ValidationError, TResponse>? BuildSafeFailureFactory()
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return static error => (TResponse)(object)Result.Failure(error);
        }

        return ValidationFailureRegistry.Get<TResponse>();
    }

    /// <summary>
    /// Resolves (and caches) the dynamic <see cref="Result{T}"/> failure factory used only when no
    /// source-generated factory is registered. Reached only on the reflection-based scan path.
    /// </summary>
    [RequiresUnreferencedCode(DynamicFallbackJustification)]
    [RequiresDynamicCode(DynamicFallbackJustification)]
    private static Func<ValidationError, TResponse>? ResolveDynamicFailureFactory()
    {
        if (_dynamicFailureFactory is not null)
        {
            return _dynamicFailureFactory;
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

            _dynamicFailureFactory = Expression.Lambda<Func<ValidationError, TResponse>>(body, errorParam).Compile();
            return _dynamicFailureFactory;
        }

        return null;
    }
}
