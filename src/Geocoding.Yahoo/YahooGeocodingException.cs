using Geocoding.Core;

namespace Geocoding.Yahoo;

/// <summary>
/// Represents an error returned by the Yahoo geocoding provider.
/// </summary>
[Obsolete("Yahoo PlaceFinder/BOSS geocoding has been discontinued. This type is retained for source compatibility only and will be removed in a future major version.")]
public class YahooGeocodingException : GeocodingException
{
    private const string DefaultMessage = "There was an error processing the geocoding request. See ErrorCode or InnerException for more information.";

    /// <summary>
    /// Gets the Yahoo error code associated with the failure.
    /// </summary>
    public YahooError ErrorCode { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="YahooGeocodingException"/> class.
    /// </summary>
    /// <param name="errorCode">The Yahoo error code.</param>
    public YahooGeocodingException(YahooError errorCode)
        : base(DefaultMessage)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YahooGeocodingException"/> class.
    /// </summary>
    /// <param name="innerException">The underlying provider exception.</param>
    public YahooGeocodingException(Exception innerException)
        : base(DefaultMessage, innerException)
    {
        ErrorCode = YahooError.UnknownError;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YahooGeocodingException"/> class.
    /// </summary>
    /// <param name="message">The provider error message.</param>
    /// <param name="innerException">The underlying provider exception.</param>
    public YahooGeocodingException(string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = YahooError.UnknownError;
    }
}
