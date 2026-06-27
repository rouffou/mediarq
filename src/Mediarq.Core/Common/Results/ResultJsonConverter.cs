using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mediarq.Core.Common.Results;

/// <summary>
/// <see cref="JsonConverterFactory"/> that lets <c>System.Text.Json</c> round-trip <see cref="Result"/>
/// and <see cref="Result{T}"/>. The default object converter cannot: <see cref="Result"/> exposes only a
/// protected constructor and <see cref="Result{T}.Value"/> throws on a failed result. This converter
/// reads and writes the raw state (<c>isSuccess</c>, <c>value</c>, <c>error</c>) and rebuilds the result
/// through its factory methods, so features such as idempotency replay and distributed caching can store
/// and restore a <see cref="Result"/> response.
/// </summary>
public sealed class ResultJsonConverterFactory : JsonConverterFactory
{
    private const string DynamicCodeJustification =
        "Serializing Result<T> composes a converter for the value type via reflection-based " +
        "System.Text.Json, which is not trimming/AOT safe. Native AOT consumers that serialize results " +
        "(e.g. distributed caching) should register a source-generated serializer instead.";

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);

        return typeToConvert == typeof(Result)
            || (typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Result<>));
    }

    /// <inheritdoc />
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = DynamicCodeJustification)]
    [UnconditionalSuppressMessage("Trimming", "IL2055", Justification = DynamicCodeJustification)]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = DynamicCodeJustification)]
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(typeToConvert);

        if (typeToConvert == typeof(Result))
        {
            return new ResultJsonConverter();
        }

        var valueType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(ResultJsonConverter<>).MakeGenericType(valueType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}

/// <summary>Converts the non-generic <see cref="Result"/> to and from JSON.</summary>
internal sealed class ResultJsonConverter : JsonConverter<Result>
{
    public override Result Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected a JSON object for Result.");
        }

        var isSuccess = false;
        var error = ResultError.None;

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var property = reader.GetString();
            reader.Read();

            if (string.Equals(property, "isSuccess", StringComparison.OrdinalIgnoreCase))
            {
                isSuccess = reader.GetBoolean();
            }
            else if (string.Equals(property, "error", StringComparison.OrdinalIgnoreCase))
            {
                error = ResultJson.ReadError(ref reader, options);
            }
            else
            {
                reader.Skip();
            }
        }

        return isSuccess ? Result.Success() : Result.Failure(error);
    }

    public override void Write(Utf8JsonWriter writer, Result value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);

        writer.WriteStartObject();
        writer.WriteBoolean("isSuccess", value.IsSuccess);
        writer.WritePropertyName("error");
        ResultJson.WriteError(writer, value.Error, options);
        writer.WriteEndObject();
    }
}

/// <summary>Converts <see cref="Result{TValue}"/> to and from JSON.</summary>
internal sealed class ResultJsonConverter<TValue> : JsonConverter<Result<TValue>>
{
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "See ResultJsonConverterFactory.")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "See ResultJsonConverterFactory.")]
    public override Result<TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected a JSON object for Result<T>.");
        }

        var isSuccess = false;
        var error = ResultError.None;
        TValue? value = default;

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var property = reader.GetString();
            reader.Read();

            if (string.Equals(property, "isSuccess", StringComparison.OrdinalIgnoreCase))
            {
                isSuccess = reader.GetBoolean();
            }
            else if (string.Equals(property, "value", StringComparison.OrdinalIgnoreCase))
            {
                value = JsonSerializer.Deserialize<TValue>(ref reader, options);
            }
            else if (string.Equals(property, "error", StringComparison.OrdinalIgnoreCase))
            {
                error = ResultJson.ReadError(ref reader, options);
            }
            else
            {
                reader.Skip();
            }
        }

        return isSuccess ? Result.Success(value!) : Result.Failure<TValue>(error);
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "See ResultJsonConverterFactory.")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "See ResultJsonConverterFactory.")]
    public override void Write(Utf8JsonWriter writer, Result<TValue> value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);

        writer.WriteStartObject();
        writer.WriteBoolean("isSuccess", value.IsSuccess);
        if (value.IsSuccess)
        {
            writer.WritePropertyName("value");
            JsonSerializer.Serialize(writer, value.Value, options);
        }

        writer.WritePropertyName("error");
        ResultJson.WriteError(writer, value.Error, options);
        writer.WriteEndObject();
    }
}

/// <summary>Shared (de)serialization of <see cref="ResultError"/> used by both result converters.</summary>
internal static class ResultJson
{
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "See ResultJsonConverterFactory.")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "See ResultJsonConverterFactory.")]
    public static ResultError ReadError(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return ResultError.None;
        }

        return JsonSerializer.Deserialize<ResultError>(ref reader, options) ?? ResultError.None;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "See ResultJsonConverterFactory.")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "See ResultJsonConverterFactory.")]
    public static void WriteError(Utf8JsonWriter writer, ResultError error, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, error, options);
}
