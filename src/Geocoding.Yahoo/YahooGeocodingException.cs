using Geocoding.Core;

namespace Geocoding.Yahoo;

/// <summary>
/// Represents an error returned by the Yahoo geocoding provider.
/// </summary>
public class YahooGeocodingException : GeocodingException
{
	const string defaultMessage = "There was an error processing the geocoding request. See ErrorCode or InnerException for more information.";

	/// <summary>
	/// Gets the Yahoo error code associated with the failure.
	/// </summary>
	public YahooError ErrorCode { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="YahooGeocodingException"/> class.
	/// </summary>
	/// <param name="errorCode">The Yahoo error code.</param>
	public YahooGeocodingException(YahooError errorCode)
		: base(defaultMessage)
	{
		this.ErrorCode = errorCode;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="YahooGeocodingException"/> class.
	/// </summary>
	/// <param name="innerException">The underlying provider exception.</param>
	public YahooGeocodingException(Exception innerException)
		: base(defaultMessage, innerException)
	{
		this.ErrorCode = YahooError.UnknownError;
	}
}