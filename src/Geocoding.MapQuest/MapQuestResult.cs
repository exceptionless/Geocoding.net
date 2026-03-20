using Newtonsoft.Json;

namespace Geocoding.MapQuest;

/// <summary>
/// Result obj returned in a collection of OSM response under the property: results
/// </summary>
public class MapQuestResult
{
    /// <summary>
    /// Gets or sets the locations returned for the query.
    /// </summary>
    [JsonProperty("locations")]
    public IList<MapQuestLocation> Locations { get; set; }

    /// <summary>
    /// Gets or sets the location originally provided in the request.
    /// </summary>
    [JsonProperty("providedLocation")]
    public MapQuestLocation ProvidedLocation { get; set; }
}
