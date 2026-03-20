namespace Geocoding.Google;

/// <summary>
/// Represents a Google component filter for geocoding requests.
/// </summary>
public class GoogleComponentFilter
{
    /// <summary>
    /// Gets or sets the serialized component filter string.
    /// </summary>
    public string ComponentFilter { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleComponentFilter"/> class.
    /// </summary>
    /// <param name="component">The Google component filter name.</param>
    /// <param name="value">The component filter value.</param>
    public GoogleComponentFilter(string component, string value)
    {
        ComponentFilter = string.Format("{0}:{1}", component, value);
    }
}
