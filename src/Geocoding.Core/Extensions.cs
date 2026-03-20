using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

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
    public static bool IsNullOrEmpty<T>(this ICollection<T> col)
    {
        return col is null || col.Count == 0;
    }

    /// <summary>
    /// Executes an action for each item in an enumerable.
    /// </summary>
    /// <typeparam name="T">The enumerable item type.</typeparam>
    /// <param name="self">The source enumerable.</param>
    /// <param name="actor">The action to execute for each item.</param>
    public static void ForEach<T>(this IEnumerable<T> self, Action<T> actor)
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

    //Universal ISO DT Converter
    private static readonly JsonConverter[] JSON_CONVERTERS = new JsonConverter[]
    {
        new IsoDateTimeConverter { DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal },
        new TolerantStringEnumConverter(),
    };

    private sealed class TolerantStringEnumConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (JsonSerializationException)
            {
                var enumType = Nullable.GetUnderlyingType(objectType) ?? objectType;
                return Enum.ToObject(enumType, 0);
            }
        }
    }

    /// <summary>
    /// Serializes an object to JSON.
    /// </summary>
    /// <param name="o">The object to serialize.</param>
    /// <returns>The JSON payload, or an empty string when the input is null.</returns>
    public static string ToJSON(this object o)
    {
        string result = null;
        if (o is not null)
            result = JsonConvert.SerializeObject(o, Formatting.Indented, JSON_CONVERTERS);
        return result ?? String.Empty;
    }

    /// <summary>
    /// Deserializes JSON into a strongly typed instance.
    /// </summary>
    /// <typeparam name="T">The destination type.</typeparam>
    /// <param name="json">The JSON payload.</param>
    /// <returns>A deserialized instance, or default value for blank input.</returns>
    public static T FromJSON<T>(this string json)
    {
        T o = default(T);
        if (!String.IsNullOrWhiteSpace(json))
            o = JsonConvert.DeserializeObject<T>(json, JSON_CONVERTERS);
        return o;
    }
}
