namespace Geocoding;

/// <summary>
/// Defines geocoding and reverse geocoding operations.
/// </summary>
public interface IGeocoder
{
	/// <summary>
	/// Geocodes a single formatted address string.
	/// </summary>
	/// <param name="address">The address to geocode.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A collection of matched addresses.</returns>
	Task<IEnumerable<Address>> GeocodeAsync(string address, CancellationToken cancellationToken = default(CancellationToken));
	/// <summary>
	/// Geocodes an address from component parts.
	/// </summary>
	/// <param name="street">Street line.</param>
	/// <param name="city">City.</param>
	/// <param name="state">State or region.</param>
	/// <param name="postalCode">Postal or zip code.</param>
	/// <param name="country">Country.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A collection of matched addresses.</returns>
	Task<IEnumerable<Address>> GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken = default(CancellationToken));

	/// <summary>
	/// Reverse geocodes from a location object.
	/// </summary>
	/// <param name="location">The location to reverse geocode.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A collection of matched addresses.</returns>
	Task<IEnumerable<Address>> ReverseGeocodeAsync(Location location, CancellationToken cancellationToken = default(CancellationToken));
	/// <summary>
	/// Reverse geocodes from latitude and longitude.
	/// </summary>
	/// <param name="latitude">Latitude in decimal degrees.</param>
	/// <param name="longitude">Longitude in decimal degrees.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A collection of matched addresses.</returns>
	Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default(CancellationToken));
}