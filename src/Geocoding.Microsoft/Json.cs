using System.Text.Json.Serialization;

namespace Geocoding.Microsoft.Json;

/// <summary>
/// Represents a Bing Maps address payload.
/// </summary>
public class Address
{
    /// <summary>
    /// Gets or sets the street address line.
    /// </summary>
    [JsonPropertyName("addressLine")]
    public string? AddressLine { get; set; }
    /// <summary>
    /// Gets or sets the primary administrative district.
    /// </summary>
    [JsonPropertyName("adminDistrict")]
    public string? AdminDistrict { get; set; }
    /// <summary>
    /// Gets or sets the secondary administrative district.
    /// </summary>
    [JsonPropertyName("adminDistrict2")]
    public string? AdminDistrict2 { get; set; }
    /// <summary>
    /// Gets or sets the country or region.
    /// </summary>
    [JsonPropertyName("countryRegion")]
    public string? CountryRegion { get; set; }
    /// <summary>
    /// Gets or sets the formatted address.
    /// </summary>
    [JsonPropertyName("formattedAddress")]
    public string? FormattedAddress { get; set; }
    /// <summary>
    /// Gets or sets the locality.
    /// </summary>
    [JsonPropertyName("locality")]
    public string? Locality { get; set; }
    /// <summary>
    /// Gets or sets the neighborhood.
    /// </summary>
    [JsonPropertyName("neighborhood")]
    public string? Neighborhood { get; set; }
    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }
}
/// <summary>
/// Represents a Bing Maps bounding box.
/// </summary>
public class BoundingBox
{
    /// <summary>
    /// Gets or sets the southern latitude.
    /// </summary>
    [JsonPropertyName("southLatitude")]
    public double SouthLatitude { get; set; }
    /// <summary>
    /// Gets or sets the western longitude.
    /// </summary>
    [JsonPropertyName("westLongitude")]
    public double WestLongitude { get; set; }
    /// <summary>
    /// Gets or sets the northern latitude.
    /// </summary>
    [JsonPropertyName("northLatitude")]
    public double NorthLatitude { get; set; }
    /// <summary>
    /// Gets or sets the eastern longitude.
    /// </summary>
    [JsonPropertyName("eastLongitude")]
    public double EastLongitude { get; set; }
}
/// <summary>
/// Represents a Bing Maps point shape.
/// </summary>
public class Point
{
    /// <summary>
    /// Gets or sets the latitude/longitude coordinates.
    /// </summary>
    [JsonPropertyName("coordinates")]
    public double[] Coordinates { get; set; } = Array.Empty<double>();
    /// <summary>
    /// Gets or sets the bounding box coordinates.
    /// </summary>
    [JsonPropertyName("boundingBox")]
    public double[] BoundingBox { get; set; } = Array.Empty<double>();
}
/// <summary>
/// Represents a Bing Maps location resource.
/// </summary>
public class Location
{
    /// <summary>
    /// Gets or sets the resource name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    /// <summary>
    /// Gets or sets the representative point.
    /// </summary>
    [JsonPropertyName("point")]
    public Point? Point { get; set; }
    /// <summary>
    /// Gets or sets the bounding box.
    /// </summary>
    [JsonPropertyName("boundingBox")]
    public BoundingBox? BoundingBox { get; set; }
    /// <summary>
    /// Gets or sets the entity type.
    /// </summary>
    [JsonPropertyName("entityType")]
    public string? EntityType { get; set; }
    /// <summary>
    /// Gets or sets the structured address.
    /// </summary>
    [JsonPropertyName("address")]
    public Address? Address { get; set; }
    /// <summary>
    /// Gets or sets the confidence level.
    /// </summary>
    [JsonPropertyName("confidence")]
    public string? Confidence { get; set; }
}
/// <summary>
/// Represents a Bing Maps resource set.
/// </summary>
public class ResourceSet
{
    /// <summary>
    /// Gets or sets the estimated total resource count.
    /// </summary>
    [JsonPropertyName("estimatedTotal")]
    public long EstimatedTotal { get; set; }
    /// <summary>
    /// Gets or sets the location resources.
    /// </summary>
    [JsonPropertyName("resources")]
    public Location[] Resources { get; set; } = Array.Empty<Location>();
}
/// <summary>
/// Represents the top-level Bing Maps response.
/// </summary>
public class Response
{
    /// <summary>
    /// Gets or sets the copyright text.
    /// </summary>
    [JsonPropertyName("copyright")]
    public string? Copyright { get; set; }
    /// <summary>
    /// Gets or sets the brand logo URI.
    /// </summary>
    [JsonPropertyName("brandLogoUri")]
    public string? BrandLogoUri { get; set; }
    /// <summary>
    /// Gets or sets the HTTP-like status code.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }
    /// <summary>
    /// Gets or sets the status description.
    /// </summary>
    [JsonPropertyName("statusDescription")]
    public string? StatusDescription { get; set; }
    /// <summary>
    /// Gets or sets the authentication result code.
    /// </summary>
    [JsonPropertyName("authenticationResultCode")]
    public string? AuthenticationResultCode { get; set; }
    /// <summary>
    /// Gets or sets the error details.
    /// </summary>
    [JsonPropertyName("errorDetails")]
    public string[]? ErrorDetails { get; set; }
    /// <summary>
    /// Gets or sets the trace identifier.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }
    /// <summary>
    /// Gets or sets the resource sets.
    /// </summary>
    [JsonPropertyName("resourceSets")]
    public ResourceSet[] ResourceSets { get; set; } = Array.Empty<ResourceSet>();
}

