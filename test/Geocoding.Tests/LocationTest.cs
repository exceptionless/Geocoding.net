using Xunit;

namespace Geocoding.Tests;

public class LocationTest
{
    [Fact]
    public void Constructor_ValidCoordinates_SetsProperties()
    {
        // Arrange
        const double lat = 85.6789;
        const double lon = 92.4517;

        // Act
        Location loc = new Location(lat, lon);

        // Assert
        Assert.Equal(lat, loc.Latitude);
        Assert.Equal(lon, loc.Longitude);
    }

    [Fact]
    public void Equals_SameCoordinates_ReturnsTrue()
    {
        // Arrange
        Location loc1 = new Location(85.6789, 92.4517);
        Location loc2 = new Location(85.6789, 92.4517);

        // Assert
        Assert.True(loc1.Equals(loc2));
        Assert.Equal(loc1.GetHashCode(), loc2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_IncludesLongitudeHash()
    {
        // Arrange
        Location loc = new Location(85.6789, 92.4517);
        int expectedHashCode = unchecked((loc.Latitude.GetHashCode() * 397) ^ loc.Longitude.GetHashCode());

        // Assert
        Assert.Equal(expectedHashCode, loc.GetHashCode());
    }

    [Fact]
    public void DistanceBetween_TwoLocations_ReturnsSameDistanceBothDirections()
    {
        // Arrange
        Location loc1 = new Location(0, 0);
        Location loc2 = new Location(40, 20);

        // Act
        Distance distance1 = loc1.DistanceBetween(loc2);
        Distance distance2 = loc2.DistanceBetween(loc1);

        // Assert
        Assert.Equal(distance1, distance2);
    }
}
