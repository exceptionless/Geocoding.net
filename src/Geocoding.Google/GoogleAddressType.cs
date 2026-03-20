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
    Unknown,
    /// <summary>The StreetAddress value.</summary>
    StreetAddress,
    /// <summary>The Route value.</summary>
    Route,
    /// <summary>The Intersection value.</summary>
    Intersection,
    /// <summary>The Political value.</summary>
    Political,
    /// <summary>The Country value.</summary>
    Country,
    /// <summary>The AdministrativeAreaLevel1 value.</summary>
    AdministrativeAreaLevel1,
    /// <summary>The AdministrativeAreaLevel2 value.</summary>
    AdministrativeAreaLevel2,
    /// <summary>The AdministrativeAreaLevel3 value.</summary>
    AdministrativeAreaLevel3,
    /// <summary>The AdministrativeAreaLevel4 value.</summary>
    AdministrativeAreaLevel4,
    /// <summary>The AdministrativeAreaLevel5 value.</summary>
    AdministrativeAreaLevel5,
    /// <summary>The AdministrativeAreaLevel6 value.</summary>
    AdministrativeAreaLevel6,
    /// <summary>The AdministrativeAreaLevel7 value.</summary>
    AdministrativeAreaLevel7,
    /// <summary>The ColloquialArea value.</summary>
    ColloquialArea,
    /// <summary>The Locality value.</summary>
    Locality,
    /// <summary>The SubLocality value.</summary>
    SubLocality,
    /// <summary>The Neighborhood value.</summary>
    Neighborhood,
    /// <summary>The Premise value.</summary>
    Premise,
    /// <summary>The Subpremise value.</summary>
    Subpremise,
    /// <summary>The PostalCode value.</summary>
    PostalCode,
    /// <summary>The NaturalFeature value.</summary>
    NaturalFeature,
    /// <summary>The Airport value.</summary>
    Airport,
    /// <summary>The Park value.</summary>
    Park,
    /// <summary>The PointOfInterest value.</summary>
    PointOfInterest,
    /// <summary>The PostBox value.</summary>
    PostBox,
    /// <summary>The StreetNumber value.</summary>
    StreetNumber,
    /// <summary>The Floor value.</summary>
    Floor,
    /// <summary>The Room value.</summary>
    Room,
    /// <summary>The PostalTown value.</summary>
    PostalTown,
    /// <summary>The Establishment value.</summary>
    Establishment,
    /// <summary>The SubLocalityLevel1 value.</summary>
    SubLocalityLevel1,
    /// <summary>The SubLocalityLevel2 value.</summary>
    SubLocalityLevel2,
    /// <summary>The SubLocalityLevel3 value.</summary>
    SubLocalityLevel3,
    /// <summary>The SubLocalityLevel4 value.</summary>
    SubLocalityLevel4,
    /// <summary>The SubLocalityLevel5 value.</summary>
    SubLocalityLevel5,
    /// <summary>The PostalCodeSuffix value.</summary>
    PostalCodeSuffix,
    /// <summary>The PostalCodePrefix value.</summary>
    PostalCodePrefix,
    /// <summary>The PlusCode value.</summary>
    PlusCode,
    /// <summary>The Landmark value.</summary>
    Landmark,
    /// <summary>The Parking value.</summary>
    Parking,
    /// <summary>The BusStation value.</summary>
    BusStation,
    /// <summary>The TrainStation value.</summary>
    TrainStation,
    /// <summary>The TransitStation value.</summary>
    TransitStation
}
