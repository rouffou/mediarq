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

    private sealed record Payload(int Number, string Text);
}
