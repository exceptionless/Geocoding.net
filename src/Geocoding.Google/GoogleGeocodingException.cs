using Geocoding.Core;

namespace Geocoding.Google;

/// <summary>
/// Represents an error returned by the Google geocoding provider.
/// </summary>
public class GoogleGeocodingException : GeocodingException
{
    private const string DEFAULT_MESSAGE = "There was an error processing the geocoding request. See Status or InnerException for more information.";

    /// <summary>
    /// Gets the Google status associated with the failure.
    /// </summary>
    public GoogleStatus Status { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleGeocodingException"/> class.
    /// </summary>
    /// <param name="status">The Google status associated with the failure.</param>
    public GoogleGeocodingException(GoogleStatus status)
        : base(DEFAULT_MESSAGE)
    {
        Status = status;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleGeocodingException"/> class.
    /// </summary>
    /// <param name="innerException">The underlying provider exception.</param>
    public GoogleGeocodingException(Exception innerException)
        : base(DEFAULT_MESSAGE, innerException)
    {
        Status = GoogleStatus.Error;
    }
}
