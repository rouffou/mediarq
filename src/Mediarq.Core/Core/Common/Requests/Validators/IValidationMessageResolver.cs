namespace Mediarq.Core.Common.Requests.Validators;

/// <summary>
/// Resolves (e.g. localizes) a validation error message before it is placed on the failed result.
/// Register an implementation (for example over <c>IStringLocalizer</c>) to translate messages or
/// treat them as resource keys; without one, messages are used as-is.
/// </summary>
public interface IValidationMessageResolver
{
    /// <summary>
    /// Resolves the message for a validation failure.
    /// </summary>
    /// <param name="propertyName">The name of the invalid property.</param>
    /// <param name="message">The raw message or resource key produced by the validator.</param>
    /// <returns>The resolved (e.g. localized) message.</returns>
    string Resolve(string propertyName, string message);
}
