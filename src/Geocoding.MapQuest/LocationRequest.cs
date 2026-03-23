using System.Text.Json.Serialization;

namespace Geocoding.MapQuest;

/// <summary>
/// Represents a location payload for MapQuest geocoding and reverse geocoding requests.
/// </summary>
public class LocationRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocationRequest"/> class.
    /// </summary>
    /// <param name="street">The street address to geocode.</param>
    public LocationRequest(string street)
    {
        Street = street;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationRequest"/> class.
    /// </summary>
    /// <param name="location">The coordinates to reverse geocode.</param>
    public LocationRequest(Location location)
    {
        Location = location;
    }

    [JsonIgnore] private string? _street;
    /// <summary>
    /// Full street address or intersection for geocoding
    /// </summary>
    [JsonPropertyName("street")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual string? Street
    {
        get { return _street; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Street can not be null or blank");

            _street = value;
        }
    }

    [JsonIgnore] private Location? _location;
    /// <summary>
    /// Latitude and longitude for reverse geocoding
    /// </summary>
    [JsonPropertyName("latLng")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual Location? Location
    {
        get { return _location; }
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            _location = value;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"street: {Street}";
    }

}
