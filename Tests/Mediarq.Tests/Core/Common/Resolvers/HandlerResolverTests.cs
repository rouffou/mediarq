using FluentAssertions;
using Mediarq.Core.Common.Resolvers;

namespace Mediarq.Tests.Core.Common.Resolvers;

public class HandlerResolverTests
{
    [Fact]
    public void CannotConstructWithNullResolver()
    {
        FluentActions.Invoking(() => new HandlerResolver(null!))
            .Should().Throw<ArgumentNullException>().WithParameterName("resolver");
    }

    [Fact]
    public void Resolve_ReturnsInstanceFromUnderlyingResolver()
    {
        var sentinel = new object();
        var resolver = new HandlerResolver(_ => sentinel);

        resolver.Resolve(typeof(string)).Should().BeSameAs(sentinel);
    }

    [Fact]
    public void ResolveAll_ReturnsHandlers_WhenEnumerableResolved()
    {
        var handlers = new object[] { new(), new() };
        var resolver = new HandlerResolver(t => t == typeof(IEnumerable<string>) ? handlers : null);

        resolver.ResolveAll(typeof(string)).Should().HaveCount(2);
    }

    [Fact]
    public void ResolveAll_ReturnsEmpty_WhenNothingResolved()
    {
        var resolver = new HandlerResolver(_ => null);

        resolver.ResolveAll(typeof(string)).Should().BeEmpty();
    }

    [Fact]
    public void ResolveAll_DoesNotFallBackToSingleHandler()
    {
        // The underlying resolver only knows the single (non-enumerable) handler. ResolveAll must
        // NOT silently return it — only IEnumerable<T> resolutions are honored.
        var single = new object();
        var resolver = new HandlerResolver(t => t == typeof(string) ? single : null);

        resolver.ResolveAll(typeof(string)).Should().BeEmpty();
    }

    [Fact]
    public void ResolveGeneric_ReturnsTypedInstance()
    {
        var sentinel = "hello";
        var resolver = new HandlerResolver(t => t == typeof(string) ? sentinel : null);

        resolver.Resolve<string>().Should().BeSameAs(sentinel);
    }

    [Fact]
    public void ResolveGeneric_ReturnsNull_WhenNotRegistered()
    {
        var resolver = new HandlerResolver(_ => null);

        resolver.Resolve<string>().Should().BeNull();
    }

    [Fact]
    public void ResolveAllGeneric_ReturnsHandlers_WhenEnumerableResolved()
    {
        var handlers = new[] { "a", "b" };
        var resolver = new HandlerResolver(t => t == typeof(IEnumerable<string>) ? handlers : null);

        resolver.ResolveAll<string>().Should().HaveCount(2).And.ContainInOrder("a", "b");
    }

    [Fact]
    public void ResolveAllGeneric_ReturnsEmpty_WhenNothingResolved()
    {
        var resolver = new HandlerResolver(_ => null);

        resolver.ResolveAll<string>().Should().BeEmpty();
    }
}
