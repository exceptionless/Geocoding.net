namespace Geocoding.Google;

/// <summary>
/// Represents a viewport returned by the Google geocoding service.
/// </summary>
public class GoogleViewport
{
    /// <summary>
    /// Gets or sets the northeast corner of the viewport.
    /// </summary>
    public Location Northeast { get; set; } = null!;
    /// <summary>
    /// Gets or sets the southwest corner of the viewport.
    /// </summary>
    public Location Southwest { get; set; } = null!;
}
