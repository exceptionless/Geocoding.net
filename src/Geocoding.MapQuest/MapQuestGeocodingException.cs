using Geocoding.Core;

namespace Geocoding.MapQuest;

/// <summary>
/// Represents an error returned by the MapQuest geocoding provider.
/// </summary>
public class MapQuestGeocodingException : GeocodingException
{
    private const string DefaultMessage = "There was an error processing the geocoding request. See InnerException for more information.";

    /// <summary>
    /// Initializes a new instance of the <see cref="MapQuestGeocodingException"/> class.
    /// </summary>
    /// <param name="innerException">The underlying provider exception.</param>
    public MapQuestGeocodingException(Exception innerException)
        : base(DefaultMessage, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MapQuestGeocodingException"/> class.
    /// </summary>
    /// <param name="message">The provider error message.</param>
    public MapQuestGeocodingException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MapQuestGeocodingException"/> class.
    /// </summary>
    /// <param name="message">The provider error message.</param>
    /// <param name="innerException">The underlying provider exception.</param>
    public MapQuestGeocodingException(string message, Exception innerException)
        : base(message, innerException) { }
}