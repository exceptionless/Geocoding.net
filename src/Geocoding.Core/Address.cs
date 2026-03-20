namespace Geocoding;

/// <summary>
/// Most basic and generic form of address.
/// Just the full address string and a lat/long
/// </summary>
public abstract class Address
{
    private string _formattedAddress = String.Empty;
    private Location _coordinates;
    private string _provider = String.Empty;

    /// <summary>
    /// Initializes a new address instance.
    /// </summary>
    /// <param name="formattedAddress">The provider formatted address string.</param>
    /// <param name="coordinates">The geocoded coordinates.</param>
    /// <param name="provider">The provider name that produced this address.</param>
    public Address(string formattedAddress, Location coordinates, string provider)
    {
        FormattedAddress = formattedAddress;
        Coordinates = coordinates;
        Provider = provider;
    }

    /// <summary>
    /// Gets or sets the full formatted address.
    /// </summary>
    public virtual string FormattedAddress
    {
        get { return _formattedAddress; }
        set
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("FormattedAddress can not be null or blank.", nameof(FormattedAddress));

            _formattedAddress = value.Trim();
        }
    }

    /// <summary>
    /// Gets or sets the latitude and longitude for this address.
    /// </summary>
    public virtual Location Coordinates
    {
        get { return _coordinates; }
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(Coordinates));

            _coordinates = value;
        }
    }

    /// <summary>
    /// Gets the provider name for this address.
    /// </summary>
    public virtual string Provider
    {
        get { return _provider; }
        protected set
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Provider can not be null or blank.", nameof(Provider));

            _provider = value;
        }
    }

    /// <summary>
    /// Calculates the distance from this address to another address in miles.
    /// </summary>
    /// <param name="address">The destination address.</param>
    /// <returns>The distance between the two addresses.</returns>
    public virtual Distance DistanceBetween(Address address)
    {
        return Coordinates.DistanceBetween(address.Coordinates);
    }

    /// <summary>
    /// Calculates the distance from this address to another address.
    /// </summary>
    /// <param name="address">The destination address.</param>
    /// <param name="units">The unit to return the distance in.</param>
    /// <returns>The distance between the two addresses.</returns>
    public virtual Distance DistanceBetween(Address address, DistanceUnits units)
    {
        return Coordinates.DistanceBetween(address.Coordinates, units);
    }

    /// <summary>
    /// Returns the formatted address.
    /// </summary>
    /// <returns>The formatted address.</returns>
    public override string ToString()
    {
        return FormattedAddress;
    }
}
