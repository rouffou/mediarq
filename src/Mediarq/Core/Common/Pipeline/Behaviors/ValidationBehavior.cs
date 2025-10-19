using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Results;
using System.ComponentModel.DataAnnotations;

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
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var failures = _validators
            .SelectMany(v => v.Validate(context.Request))
            .Where(r => !r.IsValid)
            .SelectMany(v => v.Errors)
            .ToList();

        // Aucune erreur → continuer le pipeline
        if (!failures.Any())
            return next();

        // Extraire toutes les erreurs individuelles
        var propertyErrors = failures
            .Select(e => new Error(
                $"Validation.{typeof(TRequest).Name}.{e.PropertyName}",
                e.ErrorMessage,
                ErrorType.Validation))
            .ToArray();

        // Créer un objet ValidationError global
        var validationError = new ValidationError(propertyErrors);

        // Si TResponse est un Result (non générique)
        if (typeof(TResponse) == typeof(Result))
            return Task.FromResult((TResponse)(object)Result.Failure(validationError));

        // Si TResponse est un Result<T>
        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = typeof(TResponse).GetGenericArguments()[0];
            var method = typeof(Result<>)
                .MakeGenericType(valueType)
                .GetMethod(nameof(Result<object>.ValidationFailure));

            var genericResult = method!.Invoke(null, new object[] { validationError });
            return Task.FromResult((TResponse)genericResult!);
        }

        // Sinon, type non supporté
        throw new InvalidOperationException($"Validation failed but TResponse type '{typeof(TResponse).Name}' is not a supported Result type.");
    }
}
