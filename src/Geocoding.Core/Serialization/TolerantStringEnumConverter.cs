using System.Text.Json;
using System.Text.Json.Serialization;

namespace Geocoding.Serialization;

/// <summary>
/// A <see cref="JsonConverterFactory"/> that deserializes enum values tolerantly,
/// returning an <c>Unknown</c> enum member when one exists, or the default value otherwise.
/// This prevents deserialization failures when a geocoding API returns new enum values
/// that the library doesn't yet know about while still preserving nullable-enum behavior.
/// </summary>
internal sealed class TolerantStringEnumConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum || (Nullable.GetUnderlyingType(typeToConvert)?.IsEnum == true);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var nullableEnumType = Nullable.GetUnderlyingType(typeToConvert);
        var converterType = nullableEnumType is null
            ? typeof(TolerantStringEnumConverter<>).MakeGenericType(typeToConvert)
            : typeof(NullableTolerantStringEnumConverter<>).MakeGenericType(nullableEnumType);
        return (JsonConverter)Activator.CreateInstance(converterType);
    }
}

internal sealed class TolerantStringEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    private static readonly TEnum FallbackValue = GetFallbackValue();

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt32(out int intValue))
                return Enum.IsDefined(typeof(TEnum), intValue) ? (TEnum)(object)intValue : FallbackValue;

            return FallbackValue;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (Enum.TryParse<TEnum>(value, true, out var result) && Enum.IsDefined(typeof(TEnum), result))
                return result;

            return FallbackValue;
        }

        return FallbackValue;
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    private static TEnum GetFallbackValue()
    {
        foreach (string name in Enum.GetNames(typeof(TEnum)))
        {
            if (String.Equals(name, "Unknown", StringComparison.OrdinalIgnoreCase))
                return (TEnum)Enum.Parse(typeof(TEnum), name);
        }

        return default;
    }
}

internal sealed class NullableTolerantStringEnumConverter<TEnum> : JsonConverter<TEnum?> where TEnum : struct, Enum
{
    private static readonly TolerantStringEnumConverter<TEnum> InnerConverter = new();

    public override TEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        return InnerConverter.Read(ref reader, typeof(TEnum), options);
    }

    public override void Write(Utf8JsonWriter writer, TEnum? value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        InnerConverter.Write(writer, value.Value, options);
    }
}
