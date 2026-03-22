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
    private static readonly Type EnumType = typeof(TEnum);
    private static readonly Type UnderlyingType = Enum.GetUnderlyingType(EnumType);
    private static readonly TEnum FallbackValue = GetFallbackValue();

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return TryReadNumericValue(ref reader, out var value) ? value : FallbackValue;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (Enum.TryParse<TEnum>(value, true, out var result) && Enum.IsDefined(EnumType, result))
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
        foreach (string name in Enum.GetNames(EnumType))
        {
            if (String.Equals(name, "Unknown", StringComparison.OrdinalIgnoreCase))
                return (TEnum)Enum.Parse(EnumType, name);
        }

        return default;
    }

    private static bool TryReadNumericValue(ref Utf8JsonReader reader, out TEnum value)
    {
        value = FallbackValue;

        object? rawValue = null;
        switch (Type.GetTypeCode(UnderlyingType))
        {
            case TypeCode.SByte:
                if (reader.TryGetInt64(out var sbyteValue) && sbyteValue >= sbyte.MinValue && sbyteValue <= sbyte.MaxValue)
                    rawValue = (sbyte)sbyteValue;
                break;
            case TypeCode.Byte:
                if (reader.TryGetUInt64(out var byteValue) && byteValue <= byte.MaxValue)
                    rawValue = (byte)byteValue;
                break;
            case TypeCode.Int16:
                if (reader.TryGetInt64(out var int16Value) && int16Value >= short.MinValue && int16Value <= short.MaxValue)
                    rawValue = (short)int16Value;
                break;
            case TypeCode.UInt16:
                if (reader.TryGetUInt64(out var uint16Value) && uint16Value <= ushort.MaxValue)
                    rawValue = (ushort)uint16Value;
                break;
            case TypeCode.Int32:
                if (reader.TryGetInt32(out var int32Value))
                    rawValue = int32Value;
                break;
            case TypeCode.UInt32:
                if (reader.TryGetUInt64(out var uint32Value) && uint32Value <= uint.MaxValue)
                    rawValue = (uint)uint32Value;
                break;
            case TypeCode.Int64:
                if (reader.TryGetInt64(out var int64Value))
                    rawValue = int64Value;
                break;
            case TypeCode.UInt64:
                if (reader.TryGetUInt64(out var uint64Value))
                    rawValue = uint64Value;
                break;
        }

        if (rawValue is null)
            return false;

        var enumValue = (TEnum)Enum.ToObject(EnumType, rawValue);
        if (!Enum.IsDefined(EnumType, enumValue))
            return false;

        value = enumValue;
        return true;
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
