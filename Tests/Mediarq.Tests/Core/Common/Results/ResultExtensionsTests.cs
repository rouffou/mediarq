using FluentAssertions;
using Mediarq.Core.Common.Results;

namespace Mediarq.Tests.Core.Common.Results;

public class ResultExtensionsTests
{
    private static readonly ResultError Error = ResultError.Failure("E", "boom");

    [Fact]
    public void Map_Transforms_Value_On_Success()
    {
        Result<int> result = Result.Success(2);

        result.Map(x => x * 10).Value.Should().Be(20);
    }

    [Fact]
    public void Map_Propagates_Error_On_Failure()
    {
        Result<int> result = Result.Failure<int>(Error);

        var mapped = result.Map(x => x * 10);

        mapped.IsFailure.Should().BeTrue();
        mapped.Error.Should().Be(Error);
    }

    [Fact]
    public void Bind_Chains_On_Success_And_Propagates_On_Failure()
    {
        Result.Success(2).Bind(x => Result.Success(x + 1)).Value.Should().Be(3);
        Result.Failure<int>(Error).Bind(x => Result.Success(x + 1)).IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Match_Selects_Branch()
    {
        Result.Success(5).Match(v => $"ok:{v}", e => $"err:{e.Code}").Should().Be("ok:5");
        Result.Failure<int>(Error).Match(v => $"ok:{v}", e => $"err:{e.Code}").Should().Be("err:E");
    }

    [Fact]
    public void Tap_Runs_Action_On_Success_Only()
    {
        var seen = 0;

        Result.Success(7).Tap(v => seen = v);
        seen.Should().Be(7);

        Result.Failure<int>(Error).Tap(v => seen = v);
        seen.Should().Be(7); // unchanged on failure
    }

    [Fact]
    public void Ensure_Fails_When_Predicate_Not_Met()
    {
        Result.Success(1).Ensure(x => x > 5, Error).IsFailure.Should().BeTrue();
        Result.Success(10).Ensure(x => x > 5, Error).IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task MapAsync_And_BindAsync_Work()
    {
        var mapped = await Task.FromResult(Result.Success(2)).MapAsync(x => x + 3);
        mapped.Value.Should().Be(5);

        var bound = await Task.FromResult(Result.Success(2)).BindAsync(x => Task.FromResult(Result.Success(x * 4)));
        bound.Value.Should().Be(8);
    }
}
