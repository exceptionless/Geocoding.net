using System.Diagnostics.CodeAnalysis;

namespace Geocoding.Extensions;

/// <summary>
/// Collection-related helpers.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Returns whether a collection is null or has no items.
    /// </summary>
    /// <typeparam name="T">The collection item type.</typeparam>
    /// <param name="collection">The collection to test.</param>
    /// <returns><c>true</c> when the collection is null or empty.</returns>
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this ICollection<T>? collection)
    {
        return collection is null || collection.Count == 0;
    }
}