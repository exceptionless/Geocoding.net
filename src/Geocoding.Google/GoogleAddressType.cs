namespace Geocoding.Google;

/// <summary>
/// Represents the address type returned by the Google geocoding service.
/// </summary>
/// <remarks>
/// https://developers.google.com/maps/documentation/geocoding/requests-geocoding#Types
/// </remarks>
public enum GoogleAddressType
{
    /// <summary>The Unknown value.</summary>
    Unknown = 0,
    /// <summary>The StreetAddress value.</summary>
    StreetAddress = 1,
    /// <summary>The Route value.</summary>
    Route = 2,
    /// <summary>The Intersection value.</summary>
    Intersection = 3,
    /// <summary>The Political value.</summary>
    Political = 4,
    /// <summary>The Country value.</summary>
    Country = 5,
    /// <summary>The AdministrativeAreaLevel1 value.</summary>
    AdministrativeAreaLevel1 = 6,
    /// <summary>The AdministrativeAreaLevel2 value.</summary>
    AdministrativeAreaLevel2 = 7,
    /// <summary>The AdministrativeAreaLevel3 value.</summary>
    AdministrativeAreaLevel3 = 8,
    /// <summary>The AdministrativeAreaLevel4 value.</summary>
    AdministrativeAreaLevel4 = 32,
    /// <summary>The AdministrativeAreaLevel5 value.</summary>
    AdministrativeAreaLevel5 = 33,
    /// <summary>The AdministrativeAreaLevel6 value.</summary>
    AdministrativeAreaLevel6 = 34,
    /// <summary>The AdministrativeAreaLevel7 value.</summary>
    AdministrativeAreaLevel7 = 35,
    /// <summary>The ColloquialArea value.</summary>
    ColloquialArea = 9,
    /// <summary>The Locality value.</summary>
    Locality = 10,
    /// <summary>The SubLocality value.</summary>
    SubLocality = 11,
    /// <summary>The Neighborhood value.</summary>
    Neighborhood = 12,
    /// <summary>The Premise value.</summary>
    Premise = 13,
    /// <summary>The Subpremise value.</summary>
    Subpremise = 14,
    /// <summary>The PostalCode value.</summary>
    PostalCode = 15,
    /// <summary>The NaturalFeature value.</summary>
    NaturalFeature = 16,
    /// <summary>The Airport value.</summary>
    Airport = 17,
    /// <summary>The Park value.</summary>
    Park = 18,
    /// <summary>The PointOfInterest value.</summary>
    PointOfInterest = 19,
    /// <summary>The PostBox value.</summary>
    PostBox = 20,
    /// <summary>The StreetNumber value.</summary>
    StreetNumber = 21,
    /// <summary>The Floor value.</summary>
    Floor = 22,
    /// <summary>The Room value.</summary>
    Room = 23,
    /// <summary>The PostalTown value.</summary>
    PostalTown = 24,
    /// <summary>The Establishment value.</summary>
    Establishment = 25,
    /// <summary>The SubLocalityLevel1 value.</summary>
    SubLocalityLevel1 = 26,
    /// <summary>The SubLocalityLevel2 value.</summary>
    SubLocalityLevel2 = 27,
    /// <summary>The SubLocalityLevel3 value.</summary>
    SubLocalityLevel3 = 28,
    /// <summary>The SubLocalityLevel4 value.</summary>
    SubLocalityLevel4 = 29,
    /// <summary>The SubLocalityLevel5 value.</summary>
    SubLocalityLevel5 = 30,
    /// <summary>The PostalCodeSuffix value.</summary>
    PostalCodeSuffix = 31,
    /// <summary>The PostalCodePrefix value.</summary>
    PostalCodePrefix = 36,
    /// <summary>The PlusCode value.</summary>
    PlusCode = 37,
    /// <summary>The Landmark value.</summary>
    Landmark = 38,
    /// <summary>The Parking value.</summary>
    Parking = 39,
    /// <summary>The BusStation value.</summary>
    BusStation = 40,
    /// <summary>The TrainStation value.</summary>
    TrainStation = 41,
    /// <summary>The TransitStation value.</summary>
    TransitStation = 42
}
