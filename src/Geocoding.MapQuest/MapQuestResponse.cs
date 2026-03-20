using Newtonsoft.Json;

namespace Geocoding.MapQuest;

/// <summary>
/// Represents a MapQuest geocoding response payload.
/// </summary>
public class MapQuestResponse
{
	//[JsonArray(AllowNullItems=true)]
	/// <summary>
	/// Gets or sets the result collection.
	/// </summary>
	[JsonProperty("results")]
	public IList<MapQuestResult> Results { get; set; }

	/// <summary>
	/// Gets or sets the request options echoed by MapQuest.
	/// </summary>
	[JsonProperty("options")]
	public RequestOptions Options { get; set; }

	/// <summary>
	/// Gets or sets response metadata.
	/// </summary>
	[JsonProperty("info")]
	public ResponseInfo Info { get; set; }
}