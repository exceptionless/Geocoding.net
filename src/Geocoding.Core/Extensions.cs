using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Geocoding;

/// <summary>
/// Backward-compatible entry point for shared geocoding helper extensions.
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
        return global::Geocoding.Collections.CollectionExtensions.IsNullOrEmpty(col);
    }

    /// <summary>
    /// Executes an action for each item in an enumerable.
    /// </summary>
    /// <typeparam name="T">The enumerable item type.</typeparam>
    /// <param name="self">The source enumerable.</param>
    /// <param name="actor">The action to execute for each item.</param>
    public static void ForEach<T>(this IEnumerable<T>? self, Action<T> actor)
    {
        global::Geocoding.Collections.EnumerableExtensions.ForEach(self, actor);
    }

    /// <summary>
    /// Shared serialization options used across geocoding providers.
    /// </summary>
    public static JsonSerializerOptions JsonOptions => global::Geocoding.Serialization.JsonExtensions.JsonOptions;

    /// <summary>
    /// Serializes an object to JSON.
    /// </summary>
    /// <param name="o">The object to serialize.</param>
    /// <returns>The JSON payload, or an empty string when the input is null.</returns>
    public static string ToJSON(this object? o)
    {
        return global::Geocoding.Serialization.JsonExtensions.ToJSON(o);
    }

    /// <summary>
    /// Deserializes JSON into a strongly typed instance.
    /// </summary>
    /// <typeparam name="T">The destination type.</typeparam>
    /// <param name="json">The JSON payload.</param>
    /// <returns>A deserialized instance, or default value for blank input.</returns>
    public static T? FromJSON<T>(this string? json)
    {
        return global::Geocoding.Serialization.JsonExtensions.FromJSON<T>(json);
    }
}
