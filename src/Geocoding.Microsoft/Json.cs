using System.Text.Json;
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
public class Point : Shape
{
    /// <summary>
    /// Gets or sets the latitude/longitude coordinates.
    /// </summary>
    [JsonPropertyName("coordinates")]
    public double[] Coordinates { get; set; } = Array.Empty<double>();
}
/// <summary>
/// Represents a Bing Maps location resource.
/// </summary>
public class Location : Resource
{
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
    [JsonConverter(typeof(ResourceArrayConverter))]
    public Resource[] Resources { get; set; } = Array.Empty<Resource>();

    /// <summary>
    /// Gets or sets the location resources.
    /// </summary>
    [JsonIgnore]
    public Location[] Locations
    {
        get { return Resources.OfType<Location>().ToArray(); }
        set { Resources = value?.Cast<Resource>().ToArray() ?? Array.Empty<Resource>(); }
    }
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
    /// Gets or sets the error details.
    /// </summary>
    [JsonIgnore]
    public string[]? errorDetails
    {
        get { return ErrorDetails; }
        set { ErrorDetails = value; }
    }
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

/// <summary>
/// Represents a Bing Maps response hint.
/// </summary>
public class Hint
{
    /// <summary>
    /// Gets or sets the hint type.
    /// </summary>
    [JsonPropertyName("hintType")]
    public string? HintType { get; set; }

    /// <summary>
    /// Gets or sets the hint value.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>
/// Represents a Bing Maps instruction.
/// </summary>
public class Instruction
{
    /// <summary>
    /// Gets or sets the maneuver type.
    /// </summary>
    [JsonPropertyName("maneuverType")]
    public string? ManeuverType { get; set; }

    /// <summary>
    /// Gets or sets the instruction text.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

/// <summary>
/// Represents a Bing Maps route itinerary item.
/// </summary>
public class ItineraryItem
{
    /// <summary>
    /// Gets or sets the travel mode.
    /// </summary>
    [JsonPropertyName("travelMode")]
    public string? TravelMode { get; set; }

    /// <summary>
    /// Gets or sets the travel distance.
    /// </summary>
    [JsonPropertyName("travelDistance")]
    public double TravelDistance { get; set; }

    /// <summary>
    /// Gets or sets the travel duration.
    /// </summary>
    [JsonPropertyName("travelDuration")]
    public long TravelDuration { get; set; }

    /// <summary>
    /// Gets or sets the maneuver point.
    /// </summary>
    [JsonPropertyName("maneuverPoint")]
    public Point? ManeuverPoint { get; set; }

    /// <summary>
    /// Gets or sets the instruction.
    /// </summary>
    [JsonPropertyName("instruction")]
    public Instruction? Instruction { get; set; }

    /// <summary>
    /// Gets or sets the compass direction.
    /// </summary>
    [JsonPropertyName("compassDirection")]
    public string? CompassDirection { get; set; }

    /// <summary>
    /// Gets or sets the route hints.
    /// </summary>
    [JsonPropertyName("hint")]
    public Hint[] Hint { get; set; } = Array.Empty<Hint>();

    /// <summary>
    /// Gets or sets the route warnings.
    /// </summary>
    [JsonPropertyName("warning")]
    public Warning[] Warning { get; set; } = Array.Empty<Warning>();
}

/// <summary>
/// Represents a Bing Maps route line.
/// </summary>
public class Line
{
    /// <summary>
    /// Gets or sets the points that make up the line.
    /// </summary>
    [JsonPropertyName("point")]
    public Point[] Point { get; set; } = Array.Empty<Point>();
}

/// <summary>
/// Represents a Bing Maps response link.
/// </summary>
public class Link
{
    /// <summary>
    /// Gets or sets the link role.
    /// </summary>
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    /// <summary>
    /// Gets or sets the link name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the link value.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>
/// Represents a Bing Maps resource.
/// </summary>
public class Resource
{
    /// <summary>
    /// Gets or sets the resource name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the resource identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the resource links.
    /// </summary>
    [JsonPropertyName("link")]
    public Link[] Link { get; set; } = Array.Empty<Link>();

    /// <summary>
    /// Gets or sets the resource point.
    /// </summary>
    [JsonPropertyName("point")]
    public Point? Point { get; set; }

    /// <summary>
    /// Gets or sets the resource bounding box.
    /// </summary>
    [JsonPropertyName("boundingBox")]
    public BoundingBox? BoundingBox { get; set; }
}

/// <summary>
/// Represents a Bing Maps route.
/// </summary>
public class Route : Resource
{
    /// <summary>
    /// Gets or sets the distance unit.
    /// </summary>
    [JsonPropertyName("distanceUnit")]
    public string? DistanceUnit { get; set; }

    /// <summary>
    /// Gets or sets the duration unit.
    /// </summary>
    [JsonPropertyName("durationUnit")]
    public string? DurationUnit { get; set; }

    /// <summary>
    /// Gets or sets the travel distance.
    /// </summary>
    [JsonPropertyName("travelDistance")]
    public double TravelDistance { get; set; }

    /// <summary>
    /// Gets or sets the travel duration.
    /// </summary>
    [JsonPropertyName("travelDuration")]
    public long TravelDuration { get; set; }

    /// <summary>
    /// Gets or sets the route legs.
    /// </summary>
    [JsonPropertyName("routeLegs")]
    public RouteLeg[] RouteLegs { get; set; } = Array.Empty<RouteLeg>();

    /// <summary>
    /// Gets or sets the route path.
    /// </summary>
    [JsonPropertyName("routePath")]
    public RoutePath? RoutePath { get; set; }
}

/// <summary>
/// Represents a Bing Maps route leg.
/// </summary>
public class RouteLeg
{
    /// <summary>
    /// Gets or sets the travel distance.
    /// </summary>
    [JsonPropertyName("travelDistance")]
    public double TravelDistance { get; set; }

    /// <summary>
    /// Gets or sets the travel duration.
    /// </summary>
    [JsonPropertyName("travelDuration")]
    public long TravelDuration { get; set; }

    /// <summary>
    /// Gets or sets the actual start point.
    /// </summary>
    [JsonPropertyName("actualStart")]
    public Point? ActualStart { get; set; }

    /// <summary>
    /// Gets or sets the actual end point.
    /// </summary>
    [JsonPropertyName("actualEnd")]
    public Point? ActualEnd { get; set; }

    /// <summary>
    /// Gets or sets the start location.
    /// </summary>
    [JsonPropertyName("startLocation")]
    public Location? StartLocation { get; set; }

    /// <summary>
    /// Gets or sets the end location.
    /// </summary>
    [JsonPropertyName("endLocation")]
    public Location? EndLocation { get; set; }

    /// <summary>
    /// Gets or sets the itinerary items.
    /// </summary>
    [JsonPropertyName("itineraryItems")]
    public ItineraryItem[] ItineraryItems { get; set; } = Array.Empty<ItineraryItem>();
}

/// <summary>
/// Represents a Bing Maps route path.
/// </summary>
public class RoutePath
{
    /// <summary>
    /// Gets or sets the route line.
    /// </summary>
    [JsonPropertyName("line")]
    public Line? Line { get; set; }
}

/// <summary>
/// Represents a Bing Maps shape.
/// </summary>
public class Shape
{
    /// <summary>
    /// Gets or sets the bounding box coordinates.
    /// </summary>
    [JsonPropertyName("boundingBox")]
    public double[] BoundingBox { get; set; } = Array.Empty<double>();
}

/// <summary>
/// Represents a Bing Maps warning.
/// </summary>
public class Warning
{
    /// <summary>
    /// Gets or sets the warning type.
    /// </summary>
    [JsonPropertyName("warningType")]
    public string? WarningType { get; set; }

    /// <summary>
    /// Gets or sets the warning severity.
    /// </summary>
    [JsonPropertyName("severity")]
    public string? Severity { get; set; }

    /// <summary>
    /// Gets or sets the warning value.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

internal sealed class ResourceArrayConverter : JsonConverter<Resource[]>
{
    public override Resource[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return Array.Empty<Resource>();

        using var document = JsonDocument.ParseValue(ref reader);
        if (document.RootElement.ValueKind != JsonValueKind.Array)
            return Array.Empty<Resource>();

        var resources = new List<Resource>();

        foreach (var element in document.RootElement.EnumerateArray())
        {
            var resourceType = ResolveResourceType(element);
            var resource = (Resource?)JsonSerializer.Deserialize(element.GetRawText(), resourceType, options);
            if (resource is not null)
                resources.Add(resource);
        }

        return resources.ToArray();
    }

    public override void Write(Utf8JsonWriter writer, Resource[] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var resource in value)
            JsonSerializer.Serialize(writer, resource, resource.GetType(), options);

        writer.WriteEndArray();
    }

    private static Type ResolveResourceType(JsonElement element)
    {
        if (element.TryGetProperty("address", out _) || element.TryGetProperty("entityType", out _) || element.TryGetProperty("confidence", out _))
            return typeof(Location);

        if (element.TryGetProperty("routeLegs", out _) || element.TryGetProperty("routePath", out _))
            return typeof(Route);

        return typeof(Resource);
    }
}

