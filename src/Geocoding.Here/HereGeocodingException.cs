using Geocoding.Core;

namespace Geocoding.Here;

/// <summary>
/// Represents an error returned by the HERE geocoding provider.
/// </summary>
public class HereGeocodingException : GeocodingException
{
    const string defaultMessage = "There was an error processing the geocoding request. See InnerException for more information.";

    /// <summary>
    /// Gets the HERE error type returned by the API.
    /// </summary>
    public string ErrorType { get; }

    /// <summary>
    /// Gets the HERE error subtype returned by the API.
    /// </summary>
    public string ErrorSubtype { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HereGeocodingException"/> class.
    /// </summary>
    /// <param name="innerException">The underlying provider exception.</param>
    public HereGeocodingException(Exception innerException)
        : base(defaultMessage, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HereGeocodingException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorType">The provider error type.</param>
    /// <param name="errorSubtype">The provider error subtype.</param>
    public HereGeocodingException(string message, string errorType, string errorSubtype)
        : base(message)
    {
        ErrorType = errorType;
        ErrorSubtype = errorSubtype;
    }
}
