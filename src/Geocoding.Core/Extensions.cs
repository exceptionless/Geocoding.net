using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Geocoding.Serialization;

namespace Geocoding;

/// <summary>
/// Common helper extensions used by geocoding providers.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Returns whether a collection is null or has no items.
    /// </summary>
    /// <typeparam name="T">The collection item type.</typeparam>
    /// <param name="col">The collection to test.</param>
    /// <returns><c>true</c> when the collection is null or empty.</returns>
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this ICollection<T>? col)
    {
        return col is null || col.Count == 0;
    }

    /// <summary>
    /// Executes an action for each item in an enumerable.
    /// </summary>
    /// <typeparam name="T">The enumerable item type.</typeparam>
    /// <param name="self">The source enumerable.</param>
    /// <param name="actor">The action to execute for each item.</param>
    public static void ForEach<T>(this IEnumerable<T>? self, Action<T> actor)
    {
        if (actor is null)
            throw new ArgumentNullException(nameof(actor));

        if (self is null)
            return;

        foreach (T item in self)
        {
            actor(item);
        }
    }

    private static readonly JsonSerializerOptions _jsonOptions = CreateJsonOptions();

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

    /// <summary>
    /// Shared serialization options used across geocoding providers.
    /// </summary>
    public static JsonSerializerOptions JsonOptions => _jsonOptions;

    /// <summary>
    /// Serializes an object to JSON.
    /// </summary>
    /// <param name="o">The object to serialize.</param>
    /// <returns>The JSON payload, or an empty string when the input is null.</returns>
    public static string ToJSON(this object? o)
    {
        if (o is null)
            return String.Empty;

        return JsonSerializer.Serialize(o, o.GetType(), _jsonOptions);
    }

    /// <summary>
    /// Deserializes JSON into a strongly typed instance.
    /// </summary>
    /// <typeparam name="T">The destination type.</typeparam>
    /// <param name="json">The JSON payload.</param>
    /// <returns>A deserialized instance, or default value for blank input.</returns>
    public static T? FromJSON<T>(this string json)
    {
        if (String.IsNullOrWhiteSpace(json))
            return default;

        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }
}
