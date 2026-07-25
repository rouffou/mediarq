namespace Mediarq.Testing;

/// <summary>
/// Query helpers over a <see cref="SpyMediator"/>'s recorded calls. Framework-agnostic — pair with
/// whatever assertion library the test project already uses (xunit's <c>Assert</c>, FluentAssertions,
/// ...): <c>spy.Sent&lt;CreateOrder&gt;().Should().ContainSingle(...)</c>.
/// </summary>
public static class SpyMediatorAssertions
{
    /// <summary>Every recorded request of type <typeparamref name="TRequest"/>, in dispatch order.</summary>
    public static IEnumerable<TRequest> Sent<TRequest>(this SpyMediator spy)
    {
        ArgumentNullException.ThrowIfNull(spy);
        return spy.SentRequests.OfType<TRequest>();
    }

    /// <summary><see langword="true"/> when at least one request of type <typeparamref name="TRequest"/> was sent.</summary>
    public static bool HasSent<TRequest>(this SpyMediator spy) => spy.Sent<TRequest>().Any();

    /// <summary>Every recorded notification of type <typeparamref name="TNotification"/>, in publish order.</summary>
    public static IEnumerable<TNotification> Published<TNotification>(this SpyMediator spy)
    {
        ArgumentNullException.ThrowIfNull(spy);
        return spy.PublishedNotifications.OfType<TNotification>();
    }

    /// <summary><see langword="true"/> when at least one notification of type <typeparamref name="TNotification"/> was published.</summary>
    public static bool HasPublished<TNotification>(this SpyMediator spy) => spy.Published<TNotification>().Any();
}
