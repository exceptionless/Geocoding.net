namespace Geocoding.Yahoo;

/// <summary>
/// Represents an address returned by the Yahoo geocoding service.
/// </summary>
public class YahooAddress : Address
{
    readonly string name, house, street, unit, unitType, neighborhood, city, county, countyCode, state, stateCode, postalCode, country, countryCode;
    readonly int quality;

    /// <summary>
    /// Gets the result name.
    /// </summary>
    public string Name
    {
        get { return name ?? ""; }
    }

    /// <summary>
    /// Gets the house component.
    /// </summary>
    public string House
    {
        get { return house ?? ""; }
    }

    /// <summary>
    /// Gets the street component.
    /// </summary>
    public string Street
    {
        get { return street ?? ""; }
    }

    /// <summary>
    /// Gets the unit component.
    /// </summary>
    public string Unit
    {
        get { return unit ?? ""; }
    }

    /// <summary>
    /// Gets the unit type.
    /// </summary>
    public string UnitType
    {
        get { return unitType ?? ""; }
    }

    /// <summary>
    /// Gets the neighborhood.
    /// </summary>
    public string Neighborhood
    {
        get { return neighborhood ?? ""; }
    }

    /// <summary>
    /// Gets the city.
    /// </summary>
    public string City
    {
        get { return city ?? ""; }
    }

    /// <summary>
    /// Gets the county.
    /// </summary>
    public string County
    {
        get { return county ?? ""; }
    }

    /// <summary>
    /// Gets the county code.
    /// </summary>
    public string CountyCode
    {
        get { return countyCode ?? ""; }
    }

    /// <summary>
    /// Gets the state or province.
    /// </summary>
    public string State
    {
        get { return state ?? ""; }
    }

    /// <summary>
    /// Gets the state code.
    /// </summary>
    public string StateCode
    {
        get { return stateCode ?? ""; }
    }

    /// <summary>
    /// Gets the postal code.
    /// </summary>
    public string PostalCode
    {
        get { return postalCode ?? ""; }
    }

    /// <summary>
    /// Gets the country.
    /// </summary>
    public string Country
    {
        get { return country ?? ""; }
    }

    /// <summary>
    /// Gets the country code.
    /// </summary>
    public string CountryCode
    {
        get { return countryCode ?? ""; }
    }

    /// <summary>
    /// Gets the Yahoo quality score.
    /// </summary>
    public int Quality
    {
        get { return quality; }
    }

    /// <remarks>
    /// http://developer.yahoo.com/geo/placefinder/guide/responses.html#address-quality
    /// </remarks>
    public string QualityDescription
    {
        get
        {
            switch (Quality)
            {
                case 99: return "Coordinate";
                case 90: return "POI";
                case 87: return "Address match with street match";
                case 86: return "Address mismatch with street match";
                case 85: return "Address match with street mismatch";
                case 84: return "Address mismatch with street mismatch";
                case 82: return "Intersection with street match";
                case 80: return "Intersection with street mismatch";

                case 75: return "Postal unit/segment (Zip+4 in US)";
                case 74: return "Postal unit/segment, street ignored (Zip+4 in US)";
                case 72: return "Street match";
                case 71: return "Street match, address ignored";
                case 70: return "Street mismatch";

                case 64: return "Postal zone/sector, street ignored (Zip+2 in US)";
                case 63: return "AOI";
                case 62: return "Airport";
                case 60: return "Postal district (Zip Code in US)";
                case 59: return "Postal district, street ignored (Zip Code in US)";
                case 50: return "Level4 (Neighborhood)";
                case 49: return "Level4, street ignored (Neighborhood)";
                case 40: return "Level3 (City/Town/Locality)";
                case 39: return "Level3, level4 ignored (City/Town/Locality)";
                case 30: return "Level2 (County)";
                case 29: return "Level2, level3 ignored (County)";
                case 20: return "Level1 (State/Province)";
                case 19: return "Level1, level2 ignored (State/Province)";
                case 10: return "Level0 (Country)";
                case 9: return "Level0, level1 ignored (Country)";
                case 0: return "Not an address";

                default: return "Unknown";
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YahooAddress"/> class.
    /// </summary>
    /// <param name="formattedAddress">The formatted address returned by Yahoo.</param>
    /// <param name="coordinates">The coordinates returned by Yahoo.</param>
    /// <param name="name">The result name.</param>
    /// <param name="house">The house component.</param>
    /// <param name="street">The street component.</param>
    /// <param name="unit">The unit component.</param>
    /// <param name="unitType">The unit type.</param>
    /// <param name="neighborhood">The neighborhood.</param>
    /// <param name="city">The city.</param>
    /// <param name="county">The county.</param>
    /// <param name="countyCode">The county code.</param>
    /// <param name="state">The state or province.</param>
    /// <param name="stateCode">The state code.</param>
    /// <param name="postalCode">The postal code.</param>
    /// <param name="country">The country.</param>
    /// <param name="countryCode">The country code.</param>
    /// <param name="quality">The Yahoo quality score.</param>
    public YahooAddress(string formattedAddress, Location coordinates, string name, string house, string street,
        string unit, string unitType, string neighborhood, string city, string county, string countyCode, string state,
        string stateCode, string postalCode, string country, string countryCode, int quality)
        : base(formattedAddress, coordinates, "Yahoo")
    {
        this.name = name;
        this.house = house;
        this.street = street;
        this.unit = unit;
        this.unitType = unitType;
        this.neighborhood = neighborhood;
        this.city = city;
        this.county = county;
        this.countyCode = countyCode;
        this.state = state;
        this.stateCode = stateCode;
        this.postalCode = postalCode;
        this.country = country;
        this.countryCode = countryCode;
        this.quality = quality;
    }
}
