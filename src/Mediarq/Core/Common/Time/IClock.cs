namespace Mediarq.Core.Common.Time;

/// <summary>
/// Provides an abstraction for retrieving the current UTC date and time.
/// </summary>
/// <remarks>
/// The <see cref="IClock"/> interface is commonly used to decouple time-dependent logic 
/// from system time, improving testability and consistency across the application.  
/// 
/// By injecting an implementation of <see cref="IClock"/>, components can avoid directly 
/// calling <see cref="DateTime.UtcNow"/>, making it possible to simulate different times 
/// during unit testing or to provide custom time sources.
/// </remarks>
/// <example>
/// Example usage:
/// <code>
/// public class TokenService
/// {
///     private readonly IClock _clock;
///
///     public TokenService(IClock clock)
///     {
///         _clock = clock;
///     }
///
///     public Token CreateToken()
///     {
///         var expiration = _clock.UtcNow.AddHours(1);
///         return new Token(expiration);
///     }
/// }
/// </code>
/// </example>
public interface IClock
{
    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> value representing the current time in Coordinated Universal Time (UTC).
    /// </value>
    DateTime UtcNow { get; }
}
