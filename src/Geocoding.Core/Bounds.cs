using Newtonsoft.Json;

namespace Geocoding;

/// <summary>
/// Represents a rectangular viewport defined by southwest and northeast coordinates.
/// </summary>
public class Bounds
{
    private readonly Location _southWest;
    private readonly Location _northEast;

    /// <summary>
    /// Gets the southwest corner.
    /// </summary>
    public Location SouthWest
    {
        get { return _southWest; }
    }

    /// <summary>
    /// Gets the northeast corner.
    /// </summary>
    public Location NorthEast
    {
        get { return _northEast; }
    }

    /// <summary>
    /// Initializes bounds from raw latitude and longitude values.
    /// </summary>
    /// <param name="southWestLatitude">The southwest latitude.</param>
    /// <param name="southWestLongitude">The southwest longitude.</param>
    /// <param name="northEastLatitude">The northeast latitude.</param>
    /// <param name="northEastLongitude">The northeast longitude.</param>
    public Bounds(double southWestLatitude, double southWestLongitude, double northEastLatitude, double northEastLongitude)
        : this(new Location(southWestLatitude, southWestLongitude), new Location(northEastLatitude, northEastLongitude)) { }

    /// <summary>
    /// Initializes bounds from two corner locations.
    /// </summary>
    /// <param name="southWest">The southwest corner.</param>
    /// <param name="northEast">The northeast corner.</param>
    [JsonConstructor]
    public Bounds(Location southWest, Location northEast)
    {
        if (southWest == null)
            throw new ArgumentNullException("southWest");

        if (northEast == null)
            throw new ArgumentNullException("northEast");

        if (southWest.Latitude > northEast.Latitude)
            throw new ArgumentException("southWest latitude cannot be greater than northEast latitude");

        _southWest = southWest;
        _northEast = northEast;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current bounds.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> when equal; otherwise <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as Bounds);
    }

    /// <summary>
    /// Determines whether another bounds instance is equal to the current one.
    /// </summary>
    /// <param name="bounds">The other bounds instance.</param>
    /// <returns><c>true</c> when equal; otherwise <c>false</c>.</returns>
    public bool Equals(Bounds bounds)
    {
        if (bounds == null)
            return false;

        return SouthWest.Equals(bounds.SouthWest) && NorthEast.Equals(bounds.NorthEast);
    }

    /// <summary>
    /// Returns a hash code for the current bounds.
    /// </summary>
    /// <returns>A hash code for the current bounds.</returns>
    public override int GetHashCode()
    {
        return SouthWest.GetHashCode() ^ NorthEast.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of the bounds.
    /// </summary>
    /// <returns>A string representation of the bounds.</returns>
    public override string ToString()
    {
        return $"{_southWest} | {_northEast}";
    }
}
