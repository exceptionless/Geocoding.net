using System.Text.Json.Serialization;

namespace Geocoding.MapQuest;

/// <summary>
/// Result obj returned in a collection of OSM response under the property: results
/// </summary>
public class MapQuestResult
{
    /// <summary>
    /// Gets or sets the locations returned for the query.
    /// </summary>
    [JsonPropertyName("locations")]
    public IList<MapQuestLocation>? Locations { get; set; }

    /// <summary>
    /// Gets or sets the location originally provided in the request.
    /// </summary>
    [JsonPropertyName("providedLocation")]
    public MapQuestLocation? ProvidedLocation { get; set; }
}
