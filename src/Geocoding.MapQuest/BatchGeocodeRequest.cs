using System.Text.Json.Serialization;

namespace Geocoding.MapQuest;

/// <summary>
/// Represents a batch geocoding request for MapQuest.
/// </summary>
public class BatchGeocodeRequest : BaseRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BatchGeocodeRequest"/> class.
    /// </summary>
    /// <param name="key">The MapQuest application key.</param>
    /// <param name="addresses">The addresses to geocode.</param>
    public BatchGeocodeRequest(string key, ICollection<string> addresses)
        : base(key)
    {
        if (addresses.IsNullOrEmpty())
            throw new ArgumentException("addresses can not be null or empty");

        Locations = (from l in addresses select new LocationRequest(l)).ToArray();
    }

    [JsonIgnore] private readonly List<LocationRequest> _locations = new List<LocationRequest>();
    /// <summary>
    /// Required collection of concatenated address string
    /// Note input will be hashed for uniqueness.
    /// Order is not guaranteed.
    /// </summary>
    [JsonPropertyName("locations")]
    public ICollection<LocationRequest> Locations
    {
        get { return _locations; }
        set
        {
            if (value.IsNullOrEmpty())
                throw new ArgumentNullException("Locations can not be null or empty!");

            _locations.Clear();
            (from v in value
             where v is not null
             select v).ForEach(v => _locations.Add(v));

            if (_locations.Count == 0)
                throw new InvalidOperationException("At least one valid Location is required");
        }
    }

    /// <inheritdoc />
    public override string RequestAction
    {
        get { return "batch"; }
    }
}
