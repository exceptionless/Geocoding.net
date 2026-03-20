using Newtonsoft.Json;

namespace Geocoding;

/// <summary>
/// Represents a geographic coordinate.
/// </summary>
public class Location
{
	double latitude;
	double longitude;

	/// <summary>
	/// Gets or sets the latitude in decimal degrees.
	/// </summary>
	[JsonProperty("lat")]
	public virtual double Latitude
	{
		get { return latitude; }
		set
		{
			if (value < -90 || value > 90)
				throw new ArgumentOutOfRangeException("Latitude", value, "Value must be between -90 and 90 inclusive.");

			if (double.IsNaN(value))
				throw new ArgumentException("Latitude must be a valid number.", "Latitude");

			latitude = value;
		}
	}

	/// <summary>
	/// Gets or sets the longitude in decimal degrees.
	/// </summary>
	[JsonProperty("lng")]
	public virtual double Longitude
	{
		get { return longitude; }
		set
		{
			if (value < -180 || value > 180)
				throw new ArgumentOutOfRangeException("Longitude", value, "Value must be between -180 and 180 inclusive.");

			if (double.IsNaN(value))
				throw new ArgumentException("Longitude must be a valid number.", "Longitude");

			longitude = value;
		}
	}

	/// <summary>
	/// Initializes a default location at 0,0.
	/// </summary>
	protected Location()
		: this(0, 0)
	{
	}
	/// <summary>
	/// Initializes a location with latitude and longitude.
	/// </summary>
	/// <param name="latitude">Latitude in decimal degrees.</param>
	/// <param name="longitude">Longitude in decimal degrees.</param>
	public Location(double latitude, double longitude)
	{
		Latitude = latitude;
		Longitude = longitude;
	}

	/// <summary>
	/// Converts degrees to radians.
	/// </summary>
	/// <param name="val">The value in degrees.</param>
	/// <returns>The value in radians.</returns>
	protected virtual double ToRadian(double val)
	{
		return (Math.PI / 180.0) * val;
	}

	/// <summary>
	/// Calculates distance to another location in miles.
	/// </summary>
	/// <param name="location">The destination location.</param>
	/// <returns>The distance in miles.</returns>
	public virtual Distance DistanceBetween(Location location)
	{
		return DistanceBetween(location, DistanceUnits.Miles);
	}

	/// <summary>
	/// Calculates distance to another location.
	/// </summary>
	/// <param name="location">The destination location.</param>
	/// <param name="units">The distance units.</param>
	/// <returns>The calculated distance.</returns>
	public virtual Distance DistanceBetween(Location location, DistanceUnits units)
	{
		double earthRadius = (units == DistanceUnits.Miles) ? Distance.EarthRadiusInMiles : Distance.EarthRadiusInKilometers;

		double latRadian = ToRadian(location.Latitude - this.Latitude);
		double longRadian = ToRadian(location.Longitude - this.Longitude);

		double a = Math.Pow(Math.Sin(latRadian / 2.0), 2) +
		           Math.Cos(ToRadian(this.Latitude)) *
		           Math.Cos(ToRadian(location.Latitude)) *
		           Math.Pow(Math.Sin(longRadian / 2.0), 2);

		double c = 2.0 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

		double distance = earthRadius * c;
		return new Distance(distance, units);
	}

	/// <summary>
	/// Determines whether the specified object is equal to this location.
	/// </summary>
	/// <param name="obj">The object to compare.</param>
	/// <returns><c>true</c> when equal; otherwise <c>false</c>.</returns>
	public override bool Equals(object obj)
	{
		return Equals(obj as Location);
	}

	/// <summary>
	/// Determines whether another location is equal to this location.
	/// </summary>
	/// <param name="coor">The location to compare.</param>
	/// <returns><c>true</c> when equal; otherwise <c>false</c>.</returns>
	public bool Equals(Location coor)
	{
		if (coor == null)
			return false;

		return (this.Latitude == coor.Latitude && this.Longitude == coor.Longitude);
	}

	/// <summary>
	/// Returns a hash code for this location.
	/// </summary>
	/// <returns>A hash code for this location.</returns>
	public override int GetHashCode()
	{
		return Latitude.GetHashCode() ^ Latitude.GetHashCode();
	}

	/// <summary>
	/// Returns a string representation of the location.
	/// </summary>
	/// <returns>A string representation of the location.</returns>
	public override string ToString()
	{
		return string.Format("{0}, {1}", latitude, longitude);
	}
}