using Geocoding.Core;

namespace Geocoding.Microsoft;

/// <summary>
/// Represents an error returned by the Bing Maps geocoding provider.
/// </summary>
public class BingGeocodingException : GeocodingException
{
    private const string DefaultMessage = "There was an error processing the geocoding request. See InnerException for more information.";

    /// <summary>
    /// Initializes a new instance of the <see cref="BingGeocodingException"/> class.
    /// </summary>
    /// <param name="innerException">The underlying provider exception.</param>
    public BingGeocodingException(Exception innerException)
        : base(DefaultMessage, innerException) { }
}
