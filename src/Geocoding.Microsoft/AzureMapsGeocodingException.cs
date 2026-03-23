using Geocoding.Core;

namespace Geocoding.Microsoft;

/// <summary>
/// Represents an error returned by the Azure Maps geocoding provider.
/// </summary>
public class AzureMapsGeocodingException : GeocodingException
{
    private const string DefaultMessage = "There was an error processing the Azure Maps geocoding request. See InnerException for more information.";

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureMapsGeocodingException"/> class.
    /// </summary>
    /// <param name="innerException">The underlying provider exception.</param>
    public AzureMapsGeocodingException(Exception innerException)
        : base(DefaultMessage, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureMapsGeocodingException"/> class.
    /// </summary>
    /// <param name="message">The provider error message.</param>
    public AzureMapsGeocodingException(string message)
        : base(message)
    {
    }
}
