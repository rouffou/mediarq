using System.Text.Json;
using FluentAssertions;
using Mediarq.Core.Common.Results;

namespace Mediarq.Tests.Core.Common.Results;

/// <summary>
/// Covers the System.Text.Json round-trip of <see cref="Result"/> / <see cref="Result{T}"/> used by the
/// distributed caching and idempotency-replay features.
/// </summary>
public class ResultJsonConverterTests
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    [Fact]
    public void Result_Success_RoundTrips()
    {
        var json = JsonSerializer.Serialize(Result.Success(), Options);
        var restored = JsonSerializer.Deserialize<Result>(json, Options)!;

        restored.IsSuccess.Should().BeTrue();
        restored.Error.Should().Be(ResultError.None);
    }

    [Fact]
    public void Result_Failure_RoundTrips_PreservingError()
    {
        var error = ResultError.NotFound("Order.NotFound", "Order was not found.");

        var json = JsonSerializer.Serialize(Result.Failure(error), Options);
        var restored = JsonSerializer.Deserialize<Result>(json, Options)!;

        restored.IsFailure.Should().BeTrue();
        restored.Error.Should().Be(error);
        restored.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public void ResultOfT_Success_RoundTrips_PreservingValue()
    {
        var json = JsonSerializer.Serialize(Result.Success(new Payload(42, "answer")), Options);
        var restored = JsonSerializer.Deserialize<Result<Payload>>(json, Options)!;

        restored.IsSuccess.Should().BeTrue();
        restored.Value.Should().Be(new Payload(42, "answer"));
    }

    [Fact]
    public void ResultOfT_Failure_RoundTrips_WithoutTouchingValue()
    {
        var error = ResultError.Conflict("Order.Conflict", "Already confirmed.");

        // Serializing a failed Result<T> must not access the throwing Value getter.
        var json = JsonSerializer.Serialize(Result.Failure<Payload>(error), Options);
        var restored = JsonSerializer.Deserialize<Result<Payload>>(json, Options)!;

        restored.IsFailure.Should().BeTrue();
        restored.Error.Should().Be(error);
    }

    [Fact]
    public void Read_Treats_Null_Error_As_None()
    {
        // Exercises the null-error branch of the converter without hitting the
        // "a failure must have an error" guard.
        const string json = """{"isSuccess":true,"error":null}""";

        var restored = JsonSerializer.Deserialize<Result>(json, Options)!;

        restored.IsSuccess.Should().BeTrue();
        restored.Error.Should().Be(ResultError.None);
    }

    [Fact]
    public void Read_Ignores_Unknown_Properties()
    {
        // Extra/unknown properties must be skipped, not throw.
        const string json = """{"isSuccess":true,"extra":{"nested":[1,2,3]},"error":null}""";

        var restored = JsonSerializer.Deserialize<Result>(json, Options)!;

        restored.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ResultOfT_Read_Ignores_Unknown_Properties()
    {
        const string json = """{"isSuccess":true,"value":{"number":7,"text":"x"},"unknown":42,"error":null}""";

        var restored = JsonSerializer.Deserialize<Result<Payload>>(json, Options)!;

        restored.IsSuccess.Should().BeTrue();
        restored.Value.Should().Be(new Payload(7, "x"));
    }

    [Fact]
    public void Read_Throws_When_Json_Is_Not_An_Object()
    {
        var act = () => JsonSerializer.Deserialize<Result>("[]", Options);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void ResultOfT_Read_Throws_When_Json_Is_Not_An_Object()
    {
        var act = () => JsonSerializer.Deserialize<Result<Payload>>("\"oops\"", Options);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Factory_CanConvert_Matches_Only_Result_Types()
    {
        var factory = new ResultJsonConverterFactory();

        factory.CanConvert(typeof(Result)).Should().BeTrue();
        factory.CanConvert(typeof(Result<int>)).Should().BeTrue();
        factory.CanConvert(typeof(string)).Should().BeFalse();
        factory.CanConvert(typeof(List<int>)).Should().BeFalse();
    }

    private sealed record Payload(int Number, string Text);
}
