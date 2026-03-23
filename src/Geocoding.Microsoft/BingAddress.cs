namespace Geocoding.Microsoft;

/// <summary>
/// Represents an address returned by the Bing Maps geocoding service.
/// </summary>
public class BingAddress : Address
{
    private readonly string? _addressLine, _adminDistrict, _adminDistrict2, _countryRegion, _locality, _neighborhood, _postalCode;
    private readonly EntityType _type;
    private readonly ConfidenceLevel _confidence;

    /// <summary>
    /// Gets the street address line.
    /// </summary>
    public string AddressLine
    {
        get { return _addressLine ?? ""; }
    }

    /// <summary>
    /// Gets the primary administrative district.
    /// </summary>
    public string AdminDistrict
    {
        get { return _adminDistrict ?? ""; }
    }

    /// <summary>
    /// Gets the secondary administrative district.
    /// </summary>
    public string AdminDistrict2
    {
        get { return _adminDistrict2 ?? ""; }
    }

    /// <summary>
    /// Gets the country or region.
    /// </summary>
    public string CountryRegion
    {
        get { return _countryRegion ?? ""; }
    }

    /// <summary>
    /// Gets the locality.
    /// </summary>
    public string Locality
    {
        get { return _locality ?? ""; }
    }

    /// <summary>
    /// Gets the neighborhood.
    /// </summary>
    public string Neighborhood
    {
        get { return _neighborhood ?? ""; }
    }

    /// <summary>
    /// Gets the postal code.
    /// </summary>
    public string PostalCode
    {
        get { return _postalCode ?? ""; }
    }

    /// <summary>
    /// Gets the Bing Maps entity type.
    /// </summary>
    public EntityType Type
    {
        get { return _type; }
    }

    /// <summary>
    /// Gets the Bing Maps confidence level.
    /// </summary>
    public ConfidenceLevel Confidence
    {
        get { return _confidence; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BingAddress"/> class.
    /// </summary>
    /// <param name="formattedAddress">The formatted address returned by Bing Maps.</param>
    /// <param name="coordinates">The coordinates returned by Bing Maps.</param>
    /// <param name="addressLine">The street address line.</param>
    /// <param name="adminDistrict">The primary administrative district.</param>
    /// <param name="adminDistrict2">The secondary administrative district.</param>
    /// <param name="countryRegion">The country or region.</param>
    /// <param name="locality">The locality.</param>
    /// <param name="neighborhood">The neighborhood.</param>
    /// <param name="postalCode">The postal code.</param>
    /// <param name="type">The entity type returned by Bing Maps.</param>
    /// <param name="confidence">The confidence level returned by Bing Maps.</param>
    public BingAddress(string formattedAddress, Location coordinates, string? addressLine, string? adminDistrict, string? adminDistrict2,
        string? countryRegion, string? locality, string? neighborhood, string? postalCode, EntityType type, ConfidenceLevel confidence)
        : this(formattedAddress, coordinates, addressLine, adminDistrict, adminDistrict2, countryRegion, locality, neighborhood, postalCode, type, confidence, "Bing")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BingAddress"/> class for a Microsoft geocoding provider.
    /// </summary>
    /// <param name="formattedAddress">The formatted address returned by the provider.</param>
    /// <param name="coordinates">The coordinates returned by the provider.</param>
    /// <param name="addressLine">The street address line.</param>
    /// <param name="adminDistrict">The primary administrative district.</param>
    /// <param name="adminDistrict2">The secondary administrative district.</param>
    /// <param name="countryRegion">The country or region.</param>
    /// <param name="locality">The locality.</param>
    /// <param name="neighborhood">The neighborhood.</param>
    /// <param name="postalCode">The postal code.</param>
    /// <param name="type">The provider-specific entity type.</param>
    /// <param name="confidence">The provider confidence level.</param>
    /// <param name="provider">The provider name.</param>
    protected BingAddress(string formattedAddress, Location coordinates, string? addressLine, string? adminDistrict, string? adminDistrict2,
        string? countryRegion, string? locality, string? neighborhood, string? postalCode, EntityType type, ConfidenceLevel confidence, string provider)
        : base(formattedAddress, coordinates, provider)
    {
        _addressLine = addressLine;
        _adminDistrict = adminDistrict;
        _adminDistrict2 = adminDistrict2;
        _countryRegion = countryRegion;
        _locality = locality;
        _neighborhood = neighborhood;
        _postalCode = postalCode;
        _type = type;
        _confidence = confidence;
    }
}
