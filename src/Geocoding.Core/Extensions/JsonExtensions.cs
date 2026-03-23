using System.Text.Json;
using System.Text.Json.Serialization;
namespace Geocoding.Serialization;

/// <summary>
/// JSON serialization helpers and shared serializer options.
/// </summary>
public static class JsonExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = CreateJsonOptions();

    /// <summary>
    /// Shared serialization options used across geocoding providers.
    /// </summary>
    public static JsonSerializerOptions JsonOptions => _jsonOptions;

    /// <summary>
    /// Serializes an object to JSON.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <returns>The JSON payload, or an empty string when the input is null.</returns>
    public static string ToJSON(object? value)
    {
        if (value is null)
            return String.Empty;

        return JsonSerializer.Serialize(value, value.GetType(), _jsonOptions);
    }

    /// <summary>
    /// Deserializes JSON into a strongly typed instance.
    /// </summary>
    /// <typeparam name="T">The destination type.</typeparam>
    /// <param name="json">The JSON payload.</param>
    /// <returns>A deserialized instance, or default value for blank input.</returns>
    public static T? FromJSON<T>(string? json)
    {
        if (String.IsNullOrWhiteSpace(json))
            return default;

        return JsonSerializer.Deserialize<T>(json!, _jsonOptions);
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters = { new TolerantStringEnumConverterFactory() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        options.MakeReadOnly(populateMissingResolver: true);
        return options;
    }
}