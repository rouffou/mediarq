using FluentAssertions;

namespace Mediarq.Testing.Tests;

public class FakeClockTests
{
    [Fact]
    public void UtcNow_Defaults_Near_The_Real_Current_Time()
    {
        var clock = new FakeClock();

        clock.UtcNow.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UtcNow_Is_Settable()
    {
        var pinned = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var clock = new FakeClock { UtcNow = pinned };

        clock.UtcNow.Should().Be(pinned);
    }
}
