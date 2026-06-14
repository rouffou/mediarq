namespace Mediarq.Core.Common.Time;

/// <summary>
/// Provides the system implementation of the <see cref="IClock"/> interface.
/// </summary>
/// <remarks>
/// <see cref="SystemClock"/> retrieves the current Coordinated Universal Time (UTC) 
/// directly from the system clock using <see cref="DateTime.UtcNow"/>.
/// 
/// This implementation is suitable for production environments where 
/// accurate system time is required.
/// 
/// In testing or simulation scenarios, consider using a mock or 
/// custom implementation of <see cref="IClock"/> to control time behavior.
/// </remarks>
/// <example>
/// Example usage:
/// <code>
/// IClock clock = new SystemClock();
/// Console.WriteLine($"Current UTC time: {clock.UtcNow}");
/// </code>
/// </example>
public sealed class SystemClock : IClock
{
    /// <summary>
    /// Gets the current UTC date and time from the system clock.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> value representing the current system time in UTC.
    /// </value>
    public DateTime UtcNow => DateTime.UtcNow;
}
