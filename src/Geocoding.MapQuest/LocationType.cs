namespace Geocoding.MapQuest;

/// <summary>
/// Represents the location type used by MapQuest routing results.
/// </summary>
/// <remarks>
/// https://developer.mapquest.com/documentation/api/geocoding/
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
