using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Geocoding
{
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
			return col == null || col.Count == 0;
		}

		/// <summary>
		/// Executes an action for each item in an enumerable.
		/// </summary>
		/// <typeparam name="T">The enumerable item type.</typeparam>
		/// <param name="self">The source enumerable.</param>
		/// <param name="actor">The action to execute for each item.</param>
		public static void ForEach<T>(this IEnumerable<T> self, Action<T> actor)
		{
			if (actor == null)
				throw new ArgumentNullException("actor");

			if (self == null)
				return;

			foreach (T item in self)
			{
				actor(item);
			}
		}

		//Universal ISO DT Converter
		static readonly JsonConverter[] JSON_CONVERTERS = new JsonConverter[]
		{
			new IsoDateTimeConverter { DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal },
			new StringEnumConverter(),
		};

		/// <summary>
		/// Serializes an object to JSON.
		/// </summary>
		/// <param name="o">The object to serialize.</param>
		/// <returns>The JSON payload, or an empty string when the input is null.</returns>
		public static string ToJSON(this object o)
		{
			string result = null;
			if (o != null)
				result = JsonConvert.SerializeObject(o, Formatting.Indented, JSON_CONVERTERS);
			return result ?? string.Empty;
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
			if (!string.IsNullOrWhiteSpace(json))
				o = JsonConvert.DeserializeObject<T>(json, JSON_CONVERTERS);
			return o;
		}
	}
}
