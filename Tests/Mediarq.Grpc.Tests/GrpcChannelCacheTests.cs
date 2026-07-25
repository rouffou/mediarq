using FluentAssertions;

namespace Mediarq.Grpc.Tests;

public class GrpcChannelCacheTests
{
    [Fact]
    public void GetOrCreate_Returns_The_Same_Channel_For_The_Same_Address()
    {
        using var cache = new GrpcChannelCache();

        var first = cache.GetOrCreate("https://order-service:5001");
        var second = cache.GetOrCreate("https://order-service:5001");

        second.Should().BeSameAs(first);
    }

    [Fact]
    public void GetOrCreate_Returns_Different_Channels_For_Different_Addresses()
    {
        using var cache = new GrpcChannelCache();

        var first = cache.GetOrCreate("https://order-service:5001");
        var second = cache.GetOrCreate("https://billing-service:5002");

        second.Should().NotBeSameAs(first);
    }

    [Fact]
    public void Dispose_Does_Not_Throw_When_No_Channel_Was_Ever_Created()
    {
        var cache = new GrpcChannelCache();

        var act = cache.Dispose;

        act.Should().NotThrow();
    }
}
