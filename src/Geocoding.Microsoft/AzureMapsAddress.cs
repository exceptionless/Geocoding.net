namespace Geocoding.Microsoft;

/// <summary>
/// Represents an address returned by the Azure Maps geocoding service.
/// </summary>
public class AzureMapsAddress : Address
{
    private readonly string? _addressLine, _adminDistrict, _adminDistrict2, _countryRegion, _locality, _neighborhood, _postalCode;
    private readonly EntityType _type;
    private readonly ConfidenceLevel _confidence;

    /// <summary>
    /// Gets the street address line.
    /// </summary>
    public string AddressLine => _addressLine ?? "";

    /// <summary>
    /// Gets the primary administrative district.
    /// </summary>
    public string AdminDistrict => _adminDistrict ?? "";

    /// <summary>
    /// Gets the secondary administrative district.
    /// </summary>
    public string AdminDistrict2 => _adminDistrict2 ?? "";

    /// <summary>
    /// Gets the country or region.
    /// </summary>
    public string CountryRegion => _countryRegion ?? "";

    /// <summary>
    /// Gets the locality.
    /// </summary>
    public string Locality => _locality ?? "";

    /// <summary>
    /// Gets the neighborhood.
    /// </summary>
    public string Neighborhood => _neighborhood ?? "";

    /// <summary>
    /// Gets the postal code.
    /// </summary>
    public string PostalCode => _postalCode ?? "";

    /// <summary>
    /// Gets the Azure Maps entity type.
    /// </summary>
    public EntityType Type => _type;

    /// <summary>
    /// Gets the Azure Maps confidence level.
    /// </summary>
    public ConfidenceLevel Confidence => _confidence;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureMapsAddress"/> class.
    /// </summary>
    /// <param name="formattedAddress">The formatted address returned by Azure Maps.</param>
    /// <param name="coordinates">The coordinates returned by Azure Maps.</param>
    /// <param name="addressLine">The street address line.</param>
    /// <param name="adminDistrict">The primary administrative district.</param>
    /// <param name="adminDistrict2">The secondary administrative district.</param>
    /// <param name="countryRegion">The country or region.</param>
    /// <param name="locality">The locality.</param>
    /// <param name="neighborhood">The neighborhood.</param>
    /// <param name="postalCode">The postal code.</param>
    /// <param name="type">The Azure-mapped geographic entity type.</param>
    /// <param name="confidence">The mapped confidence level.</param>
    public AzureMapsAddress(string formattedAddress, Location coordinates, string? addressLine, string? adminDistrict, string? adminDistrict2,
        string? countryRegion, string? locality, string? neighborhood, string? postalCode, EntityType type, ConfidenceLevel confidence)
        : base(formattedAddress, coordinates, "Azure Maps")
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
