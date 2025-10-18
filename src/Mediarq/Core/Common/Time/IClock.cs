namespace Mediarq.Core.Common.Time;

public interface IClock
{
    DateTime UtcNow { get; }
}
