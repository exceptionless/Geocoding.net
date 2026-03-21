using System.Text.Json;
using System.Text.Json.Serialization;

namespace Geocoding.Serialization;

/// <summary>
/// A <see cref="JsonConverterFactory"/> that deserializes enum values tolerantly,
/// returning the default value (0) when an unrecognized string is encountered.
/// This prevents deserialization failures when a geocoding API returns new enum values
/// that the library doesn't yet know about.
/// </summary>
internal sealed class TolerantStringEnumConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum || (Nullable.GetUnderlyingType(typeToConvert)?.IsEnum == true);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var enumType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
        var converterType = typeof(TolerantStringEnumConverter<>).MakeGenericType(enumType);
        return (JsonConverter)Activator.CreateInstance(converterType);
    }
}

internal sealed class TolerantStringEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt32(out int intValue))
                return Enum.IsDefined(typeof(TEnum), intValue) ? (TEnum)(object)intValue : default;

            return default;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (Enum.TryParse<TEnum>(value, true, out var result))
                return result;

            return default;
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
