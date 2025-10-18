using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Results;

namespace Mediarq.Core.Common.Pipeline.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        var failures = _validators
            .SelectMany(v => v.Validate(context.Request))
            .Where(r => !r.IsValid)
            .SelectMany(v => v.Errors)
            .ToList();

        if(failures.Any())
        {
            var errors = failures
                .Select(f => new ValidationPropertyError(f.PropertyName, f.ErrorMessage))
                .Select(ve => Result.Failure(new Error($"Validation.{typeof(TRequest)}", ve.ToString(), ErrorType.Validation)))
                .ToList();

            var resultType = typeof(TResponse);

            if(resultType == typeof(TResponse))
                return Task.FromResult((TResponse)(object)Result.Failure(ValidationError.FromResults(errors)));

            if(typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var valueType = typeof(TResponse).GetGenericArguments()[0];
                var method = typeof(Result)
                    .GetMethod(nameof(Result.Failure), new[] { typeof(IEnumerable<ValidationError>) })
                    .MakeGenericMethod(valueType);
                var genericResult = method.Invoke(null, new[] { errors });

                return Task.FromResult((TResponse) genericResult);
            }

            throw new InvalidOperationException("Validation failed and TResponse is not a Result type");
        }

        return next();
    }
}
