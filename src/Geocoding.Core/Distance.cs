namespace Geocoding;

/// <summary>
/// Represents a distance value and its units.
/// </summary>
public struct Distance
{
    /// <summary>
    /// Radius of earth in miles.
    /// </summary>
    public const double EarthRadiusInMiles = 3956.545;
    /// <summary>
    /// Radius of earth in kilometers.
    /// </summary>
    public const double EarthRadiusInKilometers = 6378.135;
    private const double ConversionConstant = 0.621371192;

    private readonly double _value;
    private readonly DistanceUnits _units;

    /// <summary>
    /// Gets the numeric distance value.
    /// </summary>
    public double Value
    {
        get { return _value; }
    }

    /// <summary>
    /// Gets the units of the distance value.
    /// </summary>
    public DistanceUnits Units
    {
        get { return _units; }
    }

    /// <summary>
    /// Initializes a new distance.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    /// <param name="units">The units for the value.</param>
    public Distance(double value, DistanceUnits units)
    {
        _value = Math.Round(value, 8);
        _units = units;
    }

    #region Helper Factory Methods

    /// <summary>
    /// Creates a distance from miles.
    /// </summary>
    /// <param name="miles">The value in miles.</param>
    /// <returns>A distance in miles.</returns>
    public static Distance FromMiles(double miles)
    {
        return new Distance(miles, DistanceUnits.Miles);
    }

    /// <summary>
    /// Creates a distance from kilometers.
    /// </summary>
    /// <param name="kilometers">The value in kilometers.</param>
    /// <returns>A distance in kilometers.</returns>
    public static Distance FromKilometers(double kilometers)
    {
        return new Distance(kilometers, DistanceUnits.Kilometers);
    }

    #endregion

    #region Unit Conversions

    private Distance ConvertUnits(DistanceUnits units)
    {
        if (_units == units) return this;

        double newValue;
        switch (units)
        {
            case DistanceUnits.Miles:
                newValue = _value * ConversionConstant;
                break;
            case DistanceUnits.Kilometers:
                newValue = _value / ConversionConstant;
                break;
            default:
                newValue = 0;
                break;
        }

        return new Distance(newValue, units);
    }

    /// <summary>
    /// Converts this value to miles.
    /// </summary>
    /// <returns>A distance in miles.</returns>
    public Distance ToMiles()
    {
        return ConvertUnits(DistanceUnits.Miles);
    }

    /// <summary>
    /// Converts this value to kilometers.
    /// </summary>
    /// <returns>A distance in kilometers.</returns>
    public Distance ToKilometers()
    {
        return ConvertUnits(DistanceUnits.Kilometers);
    }

    #endregion

    /// <summary>
    /// Determines whether the specified object is equal to this distance.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> when equal; otherwise <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    /// <summary>
    /// Determines whether another distance is equal to this distance.
    /// </summary>
    /// <param name="obj">The distance to compare.</param>
    /// <returns><c>true</c> when equal; otherwise <c>false</c>.</returns>
    public bool Equals(Distance obj)
    {
        return base.Equals(obj);
    }

    /// <summary>
    /// Determines whether another distance is equal, with optional unit normalization.
    /// </summary>
    /// <param name="obj">The distance to compare.</param>
    /// <param name="normalizeUnits">If <c>true</c>, converts the compared value to this instance units first.</param>
    /// <returns><c>true</c> when equal; otherwise <c>false</c>.</returns>
    public bool Equals(Distance obj, bool normalizeUnits)
    {
        if (normalizeUnits)
            obj = obj.ConvertUnits(Units);
        return Equals(obj);
    }

    /// <summary>
    /// Returns a hash code for this distance.
    /// </summary>
    /// <returns>A hash code for this distance.</returns>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of this distance.
    /// </summary>
    /// <returns>A string representation of this distance.</returns>
    public override string ToString()
    {
        return $"{_value} {_units}";
    }

    #region Operators

    /// <summary>
    /// Multiplies a distance by a scalar.
    /// </summary>
    /// <param name="d1">The source distance.</param>
    /// <param name="d">The multiplier.</param>
    /// <returns>The multiplied distance.</returns>
    public static Distance operator *(Distance d1, double d)
    {
        double newValue = d1.Value * d;
        return new Distance(newValue, d1.Units);
    }

    /// <summary>
    /// Adds two distances.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>The summed distance in left units.</returns>
    public static Distance operator +(Distance left, Distance right)
    {
        double newValue = left.Value + right.ConvertUnits(left.Units).Value;
        return new Distance(newValue, left.Units);
    }

    /// <summary>
    /// Subtracts one distance from another.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>The difference in left units.</returns>
    public static Distance operator -(Distance left, Distance right)
    {
        double newValue = left.Value - right.ConvertUnits(left.Units).Value;
        return new Distance(newValue, left.Units);
    }

    /// <summary>
    /// Compares two distances for equality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> when equal; otherwise <c>false</c>.</returns>
    public static bool operator ==(Distance left, Distance right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compares two distances for inequality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> when not equal; otherwise <c>false</c>.</returns>
    public static bool operator !=(Distance left, Distance right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Determines whether the left distance is less than the right distance.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> when left is less than right; otherwise <c>false</c>.</returns>
    public static bool operator <(Distance left, Distance right)
    {
        return left.Value < right.ConvertUnits(left.Units).Value;
    }

    /// <summary>
    /// Determines whether the left distance is less than or equal to the right distance.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> when left is less than or equal to right; otherwise <c>false</c>.</returns>
    public static bool operator <=(Distance left, Distance right)
    {
        return left.Value <= right.ConvertUnits(left.Units).Value;
    }

    /// <summary>
    /// Determines whether the left distance is greater than the right distance.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> when left is greater than right; otherwise <c>false</c>.</returns>
    public static bool operator >(Distance left, Distance right)
    {
        return left.Value > right.ConvertUnits(left.Units).Value;
    }

    /// <summary>
    /// Determines whether the left distance is greater than or equal to the right distance.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> when left is greater than or equal to right; otherwise <c>false</c>.</returns>
    public static bool operator >=(Distance left, Distance right)
    {
        return left.Value >= right.ConvertUnits(left.Units).Value;
    }

    /// <summary>
    /// Converts a distance to its numeric value.
    /// </summary>
    /// <param name="distance">The distance to convert.</param>
    /// <returns>The numeric distance value.</returns>
    public static implicit operator double(Distance distance)
    {
        return distance.Value;
    }

    #endregion
}
