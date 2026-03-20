using System.Runtime.Serialization;

namespace Geocoding.Here.Json;

/// <summary>
/// Represents the top-level HERE geocoding API payload.
/// </summary>
[DataContract]
public class ServerResponse
{
    /// <summary>
    /// Gets or sets the response payload.
    /// </summary>
    [DataMember(Name = "Response")]
    public Response Response { get; set; }
    /// <summary>
    /// Gets or sets the error details returned by the service.
    /// </summary>
    [DataMember(Name = "Details")]
    public string Details { get; set; }
    /// <summary>
    /// Gets or sets the error type returned by the service.
    /// </summary>
    [DataMember(Name = "type")]
    public string ErrorType { get; set; }
    /// <summary>
    /// Gets or sets the error subtype returned by the service.
    /// </summary>
    [DataMember(Name = "subtype")]
    public string ErrorSubtype { get; set; }
}

/// <summary>
/// Represents the HERE response body.
/// </summary>
[DataContract]
public class Response
{
    /// <summary>
    /// Gets or sets the collection of response views.
    /// </summary>
    [DataMember(Name = "View")]
    public View[] View { get; set; }
}

/// <summary>
/// Represents a HERE result view.
/// </summary>
[DataContract]
public class View
{
    /// <summary>
    /// Gets or sets the view identifier.
    /// </summary>
    [DataMember(Name = "ViewId")]
    public int ViewId { get; set; }
    /// <summary>
    /// Gets or sets the geocoding results in the view.
    /// </summary>
    [DataMember(Name = "Result")]
    public Result[] Result { get; set; }
}

/// <summary>
/// Represents an individual HERE geocoding result.
/// </summary>
[DataContract]
public class Result
{
    /// <summary>
    /// Gets or sets the service-reported relevance score.
    /// </summary>
    [DataMember(Name = "Relevance")]
    public float Relevance { get; set; }
    /// <summary>
    /// Gets or sets the match level.
    /// </summary>
    [DataMember(Name = "MatchLevel")]
    public string MatchLevel { get; set; }
    /// <summary>
    /// Gets or sets the match type.
    /// </summary>
    [DataMember(Name = "MatchType")]
    public string MatchType { get; set; }
    /// <summary>
    /// Gets or sets the matched location.
    /// </summary>
    [DataMember(Name = "Location")]
    public Location Location { get; set; }
}

/// <summary>
/// Represents a HERE location payload.
/// </summary>
[DataContract]
public class Location
{
    /// <summary>
    /// Gets or sets the HERE location identifier.
    /// </summary>
    [DataMember(Name = "LocationId")]
    public string LocationId { get; set; }
    /// <summary>
    /// Gets or sets the location type.
    /// </summary>
    [DataMember(Name = "LocationType")]
    public string LocationType { get; set; }
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    [DataMember(Name = "Name")]
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the display coordinate.
    /// </summary>
    [DataMember(Name = "DisplayPosition")]
    public GeoCoordinate DisplayPosition { get; set; }
    /// <summary>
    /// Gets or sets the navigation coordinate.
    /// </summary>
    [DataMember(Name = "NavigationPosition")]
    public GeoCoordinate NavigationPosition { get; set; }
    /// <summary>
    /// Gets or sets the structured address payload.
    /// </summary>
    [DataMember(Name = "Address")]
    public Address Address { get; set; }
}

/// <summary>
/// Represents a geographic coordinate in a HERE payload.
/// </summary>
[DataContract]
public class GeoCoordinate
{
    /// <summary>
    /// Gets or sets the latitude.
    /// </summary>
    [DataMember(Name = "Latitude")]
    public double Latitude { get; set; }
    /// <summary>
    /// Gets or sets the longitude.
    /// </summary>
    [DataMember(Name = "Longitude")]
    public double Longitude { get; set; }
}

/// <summary>
/// Represents a HERE geographic bounding box.
/// </summary>
[DataContract]
public class GeoBoundingBox
{
    /// <summary>
    /// Gets or sets the top-left coordinate.
    /// </summary>
    [DataMember(Name = "TopLeft")]
    public GeoCoordinate TopLeft { get; set; }
    /// <summary>
    /// Gets or sets the bottom-right coordinate.
    /// </summary>
    [DataMember(Name = "BottomRight")]
    public GeoCoordinate BottomRight { get; set; }
}

/// <summary>
/// Represents a structured HERE address.
/// </summary>
[DataContract]
public class Address
{
    /// <summary>
    /// Gets or sets the formatted label.
    /// </summary>
    [DataMember(Name = "Label")]
    public string Label { get; set; }
    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    [DataMember(Name = "Country")]
    public string Country { get; set; }
    /// <summary>
    /// Gets or sets the state or region.
    /// </summary>
    [DataMember(Name = "State")]
    public string State { get; set; }
    /// <summary>
    /// Gets or sets the county.
    /// </summary>
    [DataMember(Name = "County")]
    public string County { get; set; }
    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    [DataMember(Name = "City")]
    public string City { get; set; }
    /// <summary>
    /// Gets or sets the district.
    /// </summary>
    [DataMember(Name = "District")]
    public string District { get; set; }
    /// <summary>
    /// Gets or sets the subdistrict.
    /// </summary>
    [DataMember(Name = "Subdistrict")]
    public string Subdistrict { get; set; }
    /// <summary>
    /// Gets or sets the street name.
    /// </summary>
    [DataMember(Name = "Street")]
    public string Street { get; set; }
    /// <summary>
    /// Gets or sets the house number.
    /// </summary>
    [DataMember(Name = "HouseNumber")]
    public string HouseNumber { get; set; }
    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    [DataMember(Name = "PostalCode")]
    public string PostalCode { get; set; }
    /// <summary>
    /// Gets or sets the building name or identifier.
    /// </summary>
    [DataMember(Name = "Building")]
    public string Building { get; set; }
}
