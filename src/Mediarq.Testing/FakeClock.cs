using Mediarq.Core.Common.Time;

namespace Mediarq.Testing;

/// <summary>
/// A settable <see cref="IClock"/> for tests. Defaults to <see cref="DateTime.UtcNow"/> at construction;
/// set <see cref="UtcNow"/> to pin or advance time deterministically.
/// </summary>
public sealed class FakeClock : IClock
{
    /// <summary>Gets or sets the value returned by <see cref="IClock.UtcNow"/>.</summary>
    public DateTime UtcNow { get; set; } = DateTime.UtcNow;
}
