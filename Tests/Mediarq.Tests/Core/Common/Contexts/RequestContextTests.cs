using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace Mediarq.Tests.Core.Common.Contexts;

public class RequestContextTests
{
    private readonly RequestContext<TestRequest, Result<string>> _testClass;
    private readonly TestRequest _request;
    private readonly string _userId;
    private readonly CancellationToken _cancellationToken;

    public record TestRequest() : ICommand<Result<string>>;

    public RequestContextTests()
    {
        _request = new TestRequest();
        _userId = "TestUser";
        _cancellationToken = CancellationToken.None;
        _testClass = new RequestContext<TestRequest, Result<string>>(_request, _userId, _cancellationToken);
    }

    [Fact]
    public void CanConstruct()
    {
        var instance = new RequestContext<TestRequest, Result<string>>(_request, _userId, _cancellationToken);
        instance.Should().NotBeNull();
        instance.UserId.Should().Be(_userId);
        instance.Request.Should().Be(_request);
    }

    [Fact]
    public void ThrowsIfRequestIsNull()
    {
        Action act = () => new RequestContext<TestRequest, Result<string>>(null!, _userId);
        act.Should().Throw<ArgumentNullException>().WithParameterName("request");
    }

    [Fact]
    public void AddItem_Should_Add_KeyValue_Pair()
    {
        // Arrange
        var key = "Key1";
        var value = 123;

        // Act
        _testClass.AddItem(key, value);

        // Assert
        _testClass.Items.Should().ContainKey(key);
        _testClass.Items[key].Should().Be(value);
    }

    [Fact]
    public void AddItem_Should_Overwrite_Existing_Key()
    {
        var key = "SameKey";
        _testClass.AddItem(key, "Old");
        _testClass.AddItem(key, "New");

        _testClass.Items[key].Should().Be("New");
    }

    [Fact]
    public void AddItem_Should_Throw_When_Key_Is_NullOrEmpty()
    {
        FluentActions.Invoking(() => _testClass.AddItem(null!, new object()))
            .Should().Throw<ArgumentNullException>();

        FluentActions.Invoking(() => _testClass.AddItem("", new object()))
            .Should().Throw<ArgumentException>();

        FluentActions.Invoking(() => _testClass.AddItem("   ", new object()))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddItem_Should_Throw_When_Value_Is_Null()
    {
        FluentActions.Invoking(() => _testClass.AddItem("Key", null!))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TryGetItem_Should_Return_True_When_Key_Exists_And_Type_Matches()
    {
        // Arrange
        var key = "MyKey";
        _testClass.AddItem(key, 42);

        // Act
        var result = _testClass.TryGetItem<int>(key, out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void TryGetItem_Should_Return_False_When_Key_Not_Found()
    {
        var result = _testClass.TryGetItem<int>("Unknown", out var value);
        result.Should().BeFalse();
        value.Should().Be(default);
    }

    [Fact]
    public void TryGetItem_Should_Return_False_When_Type_Mismatch()
    {
        _testClass.AddItem("Key", "StringValue");

        var result = _testClass.TryGetItem<int>("Key", out var value);
        result.Should().BeFalse();
        value.Should().Be(default);
    }

    [Fact]
    public void RemoveItem_Should_Remove_Existing_Key()
    {
        var key = "ToRemove";
        _testClass.AddItem(key, "value");

        var removed = _testClass.RemoveItem(key);

        removed.Should().BeTrue();
        _testClass.Items.Should().NotContainKey(key);
    }

    [Fact]
    public void RemoveItem_Should_Return_False_When_Key_Does_Not_Exist()
    {
        var removed = _testClass.RemoveItem("NotExists");
        removed.Should().BeFalse();
    }

    [Fact]
    public void RequestId_And_CorrelationId_Are_Generated_On_Construction()
    {
        _testClass.RequestId.Should().NotBe(Guid.Empty);
        _testClass.CorrelationId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void StartedAt_Is_Initialized_To_Now()
    {
        var now = DateTime.UtcNow;
        _testClass.StartedAt.Should().BeCloseTo(now, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void FinishedAt_Can_Be_Set_And_Read()
    {
        var time = DateTime.UtcNow;
        _testClass.FinishedAt = time;
        _testClass.FinishedAt.Should().Be(time);
    }

    [Fact]
    public void Duration_Should_Return_Difference_When_FinishedAt_Is_Set()
    {
        var start = _testClass.StartedAt;
        var finish = start.AddSeconds(10);
        _testClass.FinishedAt = finish;

        _testClass.Duration.Should().BeCloseTo(TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public void Duration_Should_Return_Elapsed_When_FinishedAt_Not_Set()
    {
        var before = DateTime.UtcNow;
        var duration = _testClass.Duration;
        var after = DateTime.UtcNow;

        duration.Should().BeGreaterThanOrEqualTo(TimeSpan.Zero);
        duration.Should().BeLessThan(after - before + TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Items_Should_Return_ReadOnlyDictionary()
    {
        _testClass.Items.Should().BeAssignableTo<IReadOnlyDictionary<string, object>>();
    }
}