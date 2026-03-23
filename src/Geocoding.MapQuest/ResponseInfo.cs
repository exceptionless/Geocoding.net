using System.Text.Json.Serialization;

namespace Geocoding.MapQuest;

/// <summary>
/// Represents MapQuest response metadata.
/// </summary>
public class ResponseInfo
{
    /// <summary>
    /// Extended copyright info
    /// </summary>
    [JsonPropertyName("copyright")]
    public IDictionary<string, string>? Copyright { get; set; }

    /// <summary>
    /// Maps to HTTP response code generally
    /// </summary>
    [JsonPropertyName("statuscode")]
    public ResponseStatus Status { get; set; }

    /// <summary>
    /// Error or status messages if applicable
    /// </summary>
    [JsonPropertyName("messages")]
    public IList<string>? Messages { get; set; }
}
