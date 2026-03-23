using Geocoding.Core;

namespace Geocoding.Google;

/// <summary>
/// Represents an error returned by the Google geocoding provider.
/// </summary>
public class GoogleGeocodingException : GeocodingException
{
    private const string DEFAULT_MESSAGE = "There was an error processing the geocoding request. See Status or InnerException for more information.";

    private static string BuildMessage(GoogleStatus status, string? providerMessage)
    {
        if (String.IsNullOrWhiteSpace(providerMessage))
            return $"{DEFAULT_MESSAGE} Status: {status}.";

        return $"{DEFAULT_MESSAGE} Status: {status}. Provider message: {providerMessage}";
    }

    /// <summary>
    /// Gets the Google status associated with the failure.
    /// </summary>
    public GoogleStatus Status { get; private set; }

    /// <summary>
    /// Gets the provider-supplied error message when Google returns one.
    /// </summary>
    public string? ProviderMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleGeocodingException"/> class.
    /// </summary>
    /// <param name="status">The Google status associated with the failure.</param>
    public GoogleGeocodingException(GoogleStatus status)
        : this(status, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleGeocodingException"/> class.
    /// </summary>
    /// <param name="status">The Google status associated with the failure.</param>
    /// <param name="providerMessage">The optional Google provider message.</param>
    public GoogleGeocodingException(GoogleStatus status, string? providerMessage)
        : base(BuildMessage(status, providerMessage))
    {
        Status = status;
        ProviderMessage = providerMessage;
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
