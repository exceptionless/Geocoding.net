using Xunit;

namespace Geocoding.Tests;

public class DistanceTest
{
    [Fact]
    public void Constructor_ValidValues_SetsProperties()
    {
        // Arrange & Act
        Distance distance = new Distance(5.7, DistanceUnits.Miles);

        // Assert
        Assert.Equal(5.7, distance.Value);
        Assert.Equal(DistanceUnits.Miles, distance.Units);
    }

    [Fact]
    public void Constructor_LongDecimalValue_RoundsToEightPlaces()
    {
        // Act
        Distance distance = new Distance(0.123456789101112131415, DistanceUnits.Miles);

        // Assert
        Assert.Equal(0.12345679, distance.Value);
    }

    [Fact]
    public void Equals_SameValueAndUnits_ReturnsTrue()
    {
        // Arrange
        Distance distance1 = new Distance(5, DistanceUnits.Miles);
        Distance distance2 = new Distance(5, DistanceUnits.Miles);

        // Assert
        Assert.True(distance1.Equals(distance2));
        Assert.Equal(distance1.GetHashCode(), distance2.GetHashCode());
    }

    [Theory]
    [InlineData(1, 1.609344)]
    [InlineData(0.621371192, 1)]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    [InlineData(5, 6)]
    public void Equals_NormalizedUnits_ReturnsExpectedResult(double miles, double kilometers)
    {
        // Arrange
        Distance mileDistance = Distance.FromMiles(miles);
        Distance kilometerDistance = Distance.FromKilometers(kilometers);

        // Act
        bool expected = mileDistance.Equals(kilometerDistance.ToMiles());
        bool actual = mileDistance.Equals(kilometerDistance, true);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(-5, -8.04672)]
    [InlineData(0, 0)]
    [InlineData(1, 1.609344)]
    [InlineData(5, 8.04672)]
    [InlineData(10, 16.09344001)]
    public void ToKilometers_FromMiles_ReturnsExpectedValue(double miles, double expectedKilometers)
    {
        // Arrange
        Distance mileDistance = Distance.FromMiles(miles);

        // Act
        Distance kilometerDistance = mileDistance.ToKilometers();

        // Assert
        Assert.Equal(expectedKilometers, kilometerDistance.Value);
        Assert.Equal(DistanceUnits.Kilometers, kilometerDistance.Units);
    }

    [Theory]
    [InlineData(-5, -3.10685596)]
    [InlineData(0, 0)]
    [InlineData(1, 0.62137119)]
    [InlineData(5, 3.10685596)]
    [InlineData(10, 6.21371192)]
    public void ToMiles_FromKilometers_ReturnsExpectedValue(double kilometers, double expectedMiles)
    {
        // Arrange
        Distance kilometerDistance = Distance.FromKilometers(kilometers);

        // Act
        Distance mileDistance = kilometerDistance.ToMiles();

        // Assert
        Assert.Equal(expectedMiles, mileDistance.Value);
        Assert.Equal(DistanceUnits.Miles, mileDistance.Units);
    }

    #region Operator Tests

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0.45359237)]
    [InlineData(9, 5)]
    [InlineData(5, -5)]
    [InlineData(3, 0)]
    public void MultiplyOperator_TwoValues_ReturnsExpectedResult(double value, double multiplier)
    {
        // Arrange
        Distance distance1 = Distance.FromMiles(value);
        Distance expected = Distance.FromMiles(value * multiplier);

        // Act
        Distance actual = distance1 * multiplier;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0.45359237)]
    [InlineData(9, 5)]
    [InlineData(5, -5)]
    [InlineData(3, 0)]
    public void AddOperator_SameUnits_ReturnsExpectedResult(double left, double right)
    {
        // Arrange
        Distance distance1 = Distance.FromMiles(left);
        Distance distance2 = Distance.FromMiles(right);
        Distance expected = Distance.FromMiles(left + right);

        // Act
        Distance actual = distance1 + distance2;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0.45359237)]
    [InlineData(9, 5)]
    [InlineData(5, -5)]
    [InlineData(3, 0)]
    public void SubtractOperator_SameUnits_ReturnsExpectedResult(double left, double right)
    {
        // Arrange
        Distance distance1 = Distance.FromMiles(left);
        Distance distance2 = Distance.FromMiles(right);
        Distance expected = Distance.FromMiles(left - right);

        // Act
        Distance actual = distance1 - distance2;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0.45359237)]
    [InlineData(5, -5)]
    [InlineData(3, 3)]
    [InlineData(3.8, 3.8)]
    public void EqualityOperator_TwoValues_ReturnsExpectedResult(double left, double right)
    {
        // Arrange
        Distance distance1 = Distance.FromMiles(left);
        Distance distance2 = Distance.FromMiles(right);

        // Act & Assert
        bool expectedEqual = left == right;
        Assert.Equal(expectedEqual, distance1 == distance2);
        Assert.Equal(!expectedEqual, distance1 != distance2);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0.45359237)]
    [InlineData(9, 5)]
    [InlineData(5, -5)]
    [InlineData(3, 0)]
    public void LessThanOperator_SameUnits_ReturnsExpectedResult(double left, double right)
    {
        // Arrange
        Distance distance1 = Distance.FromMiles(left);
        Distance distance2 = Distance.FromMiles(right);

        // Act & Assert
        bool expected = left < right;
        Assert.Equal(expected, distance1 < distance2);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0.45359237)]
    [InlineData(9, 5)]
    [InlineData(5, -5)]
    [InlineData(3, 0)]
    public void LessThanOrEqualOperator_SameUnits_ReturnsExpectedResult(double left, double right)
    {
        // Arrange
        Distance distance1 = Distance.FromMiles(left);
        Distance distance2 = Distance.FromMiles(right);

        // Act & Assert
        bool expected = left <= right;
        Assert.Equal(expected, distance1 <= distance2);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0.45359237)]
    [InlineData(9, 5)]
    [InlineData(5, -5)]
    [InlineData(3, 0)]
    public void GreaterThanOperator_SameUnits_ReturnsExpectedResult(double left, double right)
    {
        // Arrange
        Distance distance1 = Distance.FromMiles(left);
        Distance distance2 = Distance.FromMiles(right);

        // Act & Assert
        bool expected = left > right;
        Assert.Equal(expected, distance1 > distance2);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0.45359237)]
    [InlineData(9, 5)]
    [InlineData(5, -5)]
    [InlineData(3, 0)]
    public void GreaterThanOrEqualOperator_SameUnits_ReturnsExpectedResult(double left, double right)
    {
        // Arrange
        Distance distance1 = Distance.FromMiles(left);
        Distance distance2 = Distance.FromMiles(right);

        // Act & Assert
        bool expected = left >= right;
        Assert.Equal(expected, distance1 >= distance2);
    }

    [Fact]
    public void ImplicitConversion_ToDouble_ReturnsValue()
    {
        Distance distance = Distance.FromMiles(56);
        double d = distance;
        Assert.Equal(d, distance.Value);
    }

    #endregion

    #region Operator Conversion Tests

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0.45359237)]
    [InlineData(9, 5)]
    [InlineData(5, -5)]
    [InlineData(3, 0)]
    public void AddOperator_DifferentUnits_ConvertsAndReturnsExpectedResult(double left, double right)
    {
        // Arrange
        Distance distance1 = Distance.FromMiles(left);
        Distance distance2 = Distance.FromKilometers(right);
        Distance expected = distance1 + distance2.ToMiles();

        // Act
        Distance actual = distance1 + distance2;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0.45359237)]
    [InlineData(9, 5)]
    [InlineData(5, -5)]
    [InlineData(3, 0)]
    public void SubtractOperator_DifferentUnits_ConvertsAndReturnsExpectedResult(double left, double right)
    {
        // Arrange
        Distance distance1 = Distance.FromMiles(left);
        Distance distance2 = Distance.FromKilometers(right);
        Distance expected = distance1 - distance2.ToMiles();

        // Act
        Distance actual = distance1 - distance2;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0.45359237)]
    [InlineData(9, 5)]
    [InlineData(5, -5)]
    [InlineData(3, 0)]
    public void LessThanOperator_DifferentUnits_ReturnsExpectedResult(double left, double right)
    {
        // Arrange
        Distance distance1 = Distance.FromMiles(left);
        Distance distance2 = Distance.FromKilometers(right);

        // Act & Assert
        bool expected = distance1 < distance2.ToMiles();
        Assert.Equal(expected, distance1 < distance2);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0.45359237)]
    [InlineData(9, 5)]
    [InlineData(5, -5)]
    [InlineData(3, 0)]
    public void LessThanOrEqualOperator_DifferentUnits_ReturnsExpectedResult(double left, double right)
    {
        // Arrange
        Distance distance1 = Distance.FromMiles(left);
        Distance distance2 = Distance.FromKilometers(right);

        // Act & Assert
        bool expected = distance1 <= distance2.ToMiles();
        Assert.Equal(expected, distance1 <= distance2);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0.45359237)]
    [InlineData(9, 5)]
    [InlineData(5, -5)]
    [InlineData(3, 0)]
    public void GreaterThanOperator_DifferentUnits_ReturnsExpectedResult(double left, double right)
    {
        // Arrange
        Distance distance1 = Distance.FromMiles(left);
        Distance distance2 = Distance.FromKilometers(right);

        // Act & Assert
        bool expected = distance1 > distance2.ToMiles();
        Assert.Equal(expected, distance1 > distance2);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0.45359237)]
    [InlineData(9, 5)]
    [InlineData(5, -5)]
    [InlineData(3, 0)]
    public void GreaterThanOrEqualOperator_DifferentUnits_ReturnsExpectedResult(double left, double right)
    {
        // Arrange
        Distance distance1 = Distance.FromMiles(left);
        Distance distance2 = Distance.FromKilometers(right);

        // Act & Assert
        bool expected = distance1 >= distance2.ToMiles();
        Assert.Equal(expected, distance1 >= distance2);
    }

    #endregion
}
