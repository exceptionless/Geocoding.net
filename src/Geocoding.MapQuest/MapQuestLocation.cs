using System.Text;
using System.Text.Json.Serialization;

namespace Geocoding.MapQuest;

/// <summary>
/// MapQuest address object.
/// See https://developer.mapquest.com/documentation/api/geocoding/.
/// </summary>
public class MapQuestLocation : ParsedAddress
{
    private const string Unknown = "unknown";
    private static readonly string DEFAULT_LOC = new Location(0, 0).ToString();

    /// <summary>
    /// Initializes a new instance of the <see cref="MapQuestLocation"/> class for deserialization.
    /// </summary>
    [JsonConstructor]
    protected MapQuestLocation() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MapQuestLocation"/> class.
    /// </summary>
    /// <param name="formattedAddress">The formatted address.</param>
    /// <param name="coordinates">The coordinates.</param>
    public MapQuestLocation(string formattedAddress, Location coordinates)
        : base(
            String.IsNullOrWhiteSpace(formattedAddress) ? Unknown : formattedAddress,
            coordinates ?? new Location(0, 0),
            "MapQuest")
    {
        DisplayCoordinates = coordinates;
    }

    /// <inheritdoc />
    [JsonPropertyName("location")]
    public override string FormattedAddress
    {
        get
        {
            return ToString();
        }
        set
        {
            if (!String.IsNullOrWhiteSpace(value))
                base.FormattedAddress = value;
        }
    }

    /// <inheritdoc />
    [JsonPropertyName("latLng")]
    public override Location Coordinates
    {
        get { return base.Coordinates; }
        set { base.Coordinates = value; }
    }

    /// <summary>
    /// Gets or sets the display coordinates.
    /// </summary>
    [JsonPropertyName("displayLatLng")]
    public virtual Location? DisplayCoordinates { get; set; }

    /// <inheritdoc />
    [JsonPropertyName("street")]
    public override string? Street { get; set; }

    /// <inheritdoc />
    [JsonPropertyName("adminArea5")]
    public override string? City { get; set; }

    /// <inheritdoc />
    [JsonPropertyName("adminArea4")]
    public override string? County { get; set; }

    /// <inheritdoc />
    [JsonPropertyName("adminArea3")]
    public override string? State { get; set; }

    /// <inheritdoc />
    [JsonPropertyName("adminArea1")]
    public override string? Country { get; set; }

    /// <inheritdoc />
    [JsonPropertyName("postalCode")]
    public override string? PostCode { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        string baseAddress = base.FormattedAddress;
        if (!String.IsNullOrEmpty(baseAddress) && baseAddress != Unknown)
            return baseAddress;
        else
        {
            var sb = new StringBuilder();
            if (!String.IsNullOrWhiteSpace(Street))
                sb.AppendFormat("{0}, ", Street);

            if (!String.IsNullOrWhiteSpace(City))
                sb.AppendFormat("{0}, ", City);

            if (!String.IsNullOrWhiteSpace(State))
                sb.AppendFormat("{0} ", State);
            else if (!String.IsNullOrWhiteSpace(County))
                sb.AppendFormat("{0} ", County);

            if (!String.IsNullOrWhiteSpace(PostCode))
                sb.AppendFormat("{0} ", PostCode);

            if (!String.IsNullOrWhiteSpace(Country))
                sb.AppendFormat("{0} ", Country);

            if (sb.Length > 1)
            {
                sb.Length--;

                string s = sb.ToString();
                if (s.Last() == ',')
                    s = s.Remove(s.Length - 1);

                return s;
            }
            else if (Coordinates is not null && Coordinates.ToString() != DEFAULT_LOC)
                return Coordinates.ToString();
            else
                return Unknown;
        }
    }

    /// <summary>
    /// Type of location
    /// </summary>
    [JsonPropertyName("type")]
    public virtual LocationType Type { get; set; }

    /// <summary>
    /// Granularity code of quality or accuracy guarantee.
    /// See https://developer.mapquest.com/documentation/api/geocoding/.
    /// </summary>
    [JsonPropertyName("geocodeQuality")]
    public virtual Quality Quality { get; set; }

    /// <summary>
    /// Text string comparable, sortable score.
    /// See https://developer.mapquest.com/documentation/api/geocoding/.
    /// </summary>
    [JsonPropertyName("geocodeQualityCode")]
    public virtual string? Confidence { get; set; }

    /// <summary>
    /// Identifies the closest road to the address for routing purposes.
    /// </summary>
    [JsonPropertyName("linkId")]
    public virtual string? LinkId { get; set; }

    /// <summary>
    /// Which side of the street this address is in
    /// </summary>
    [JsonPropertyName("sideOfStreet")]
    public virtual SideOfStreet SideOfStreet { get; set; }

    /// <summary>
    /// Url to a MapQuest map
    /// </summary>
    [JsonPropertyName("mapUrl")]
    public virtual Uri? MapUrl { get; set; }

    /// <summary>
    /// Gets or sets the country label returned by MapQuest.
    /// </summary>
    [JsonPropertyName("adminArea1Type")]
    public virtual string? CountryLabel { get; set; }

    /// <summary>
    /// Gets or sets the state label returned by MapQuest.
    /// </summary>
    [JsonPropertyName("adminArea3Type")]
    public virtual string? StateLabel { get; set; }
}
