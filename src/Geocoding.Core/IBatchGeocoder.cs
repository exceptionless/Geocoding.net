using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Geocoding
{
	/// <summary>
	/// Defines batch geocoding operations.
	/// </summary>
	public interface IBatchGeocoder
	{
		/// <summary>
		/// Geocodes a batch of addresses.
		/// </summary>
		/// <param name="addresses">The addresses to geocode.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A result item per input address.</returns>
		Task<IEnumerable<ResultItem>> GeocodeAsync(IEnumerable<string> addresses, CancellationToken cancellationToken = default(CancellationToken));
		/// <summary>
		/// Reverse geocodes a batch of coordinates.
		/// </summary>
		/// <param name="locations">The locations to reverse geocode.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A result item per input location.</returns>
		Task<IEnumerable<ResultItem>> ReverseGeocodeAsync(IEnumerable<Location> locations, CancellationToken cancellationToken = default(CancellationToken));
	}
}
