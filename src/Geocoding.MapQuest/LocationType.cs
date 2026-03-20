namespace Geocoding.MapQuest;

/// <summary>
/// Represents the location type used by MapQuest routing results.
/// </summary>
/// <remarks>
/// http://code.google.com/apis/maps/documentation/geocoding/#Types
/// </remarks>
public enum LocationType
{
    /// <summary>
    /// Stop: default
    /// </summary>
    s,
    /// <summary>
    /// Via
    /// </summary>
    v,
}
