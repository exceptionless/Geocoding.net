namespace Geocoding.Collections;

/// <summary>
/// Enumerable-related helpers.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Executes an action for each item in an enumerable.
    /// </summary>
    /// <typeparam name="T">The enumerable item type.</typeparam>
    /// <param name="source">The source enumerable.</param>
    /// <param name="actor">The action to execute for each item.</param>
    public static void ForEach<T>(IEnumerable<T>? source, Action<T> actor)
    {
        if (actor is null)
            throw new ArgumentNullException(nameof(actor));

        if (source is null)
            return;

        foreach (T item in source)
        {
            actor(item);
        }
    }
}