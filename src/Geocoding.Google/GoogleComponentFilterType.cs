namespace Geocoding.Google;

/// <summary>
/// Defines supported Google component filter names.
/// </summary>
public struct GoogleComponentFilterType
{
    /// <summary>The route component filter.</summary>
    public const string Route = "route";
    /// <summary>The locality component filter.</summary>
    public const string Locality = "locality";
    /// <summary>The administrative area component filter.</summary>
    public const string AdministrativeArea = "administrative_area";
    /// <summary>The postal code component filter.</summary>
    public const string PostalCode = "postal_code";
    /// <summary>The country component filter.</summary>
    public const string Country = "country";
}
