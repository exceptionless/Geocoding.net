namespace Geocoding.Google;

/// <summary>
/// Represents the location type returned by the Google geocoding service.
/// </summary>
/// <remarks>
/// https://developers.google.com/maps/documentation/geocoding/requests-geocoding#GeocodingResults
/// </remarks>
public enum GoogleLocationType
{
    /// <summary>The Unknown value.</summary>
    Unknown,
    /// <summary>The Rooftop value.</summary>
    Rooftop,
    /// <summary>The RangeInterpolated value.</summary>
    RangeInterpolated,
    /// <summary>The GeometricCenter value.</summary>
    GeometricCenter,
    /// <summary>The Approximate value.</summary>
    Approximate
}
