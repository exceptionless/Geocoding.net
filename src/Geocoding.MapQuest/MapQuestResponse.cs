using System.Text.Json.Serialization;

namespace Geocoding.MapQuest;

/// <summary>
/// Represents a MapQuest geocoding response payload.
/// </summary>
public class MapQuestResponse
{
    /// <summary>
    /// Gets or sets the result collection.
    /// </summary>
    [JsonPropertyName("results")]
    public IList<MapQuestResult>? Results { get; set; }

    /// <summary>
    /// Gets or sets the request options echoed by MapQuest.
    /// </summary>
    [JsonPropertyName("options")]
    public RequestOptions? Options { get; set; }

    /// <summary>
    /// Gets or sets response metadata.
    /// </summary>
    [JsonPropertyName("info")]
    public ResponseInfo? Info { get; set; }
}
