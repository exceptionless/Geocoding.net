using System;

namespace Geocoding.Core
{
	/// <summary>
	/// Base exception used for geocoding failures.
	/// </summary>
	public class GeocodingException : Exception
	{
		/// <summary>
		/// Initializes a new geocoding exception.
		/// </summary>
		/// <param name="message">The exception message.</param>
		/// <param name="innerException">The inner exception.</param>
		public GeocodingException(string message, Exception innerException = null)
			: base(message, innerException)
		{
		}
	}
}
