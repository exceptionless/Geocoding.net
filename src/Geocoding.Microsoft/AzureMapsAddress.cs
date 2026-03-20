namespace Geocoding.Microsoft;

/// <summary>
/// Represents an address returned by the Azure Maps geocoding service.
/// </summary>
public class AzureMapsAddress : BingAddress
{
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
    public AzureMapsAddress(string formattedAddress, Location coordinates, string addressLine, string adminDistrict, string adminDistrict2,
        string countryRegion, string locality, string neighborhood, string postalCode, EntityType type, ConfidenceLevel confidence)
        : base(formattedAddress, coordinates, addressLine, adminDistrict, adminDistrict2, countryRegion, locality, neighborhood, postalCode, type, confidence, "Azure Maps")
    {
    }
}
