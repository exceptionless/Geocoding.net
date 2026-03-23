using System.Text.Json;
using Geocoding.Extensions;
using Geocoding.Microsoft;
using Geocoding.Microsoft.Json;
using Xunit;

namespace Geocoding.Tests;

public class MicrosoftJsonCompatibilityTest
{
  [Fact]
  public void EntityType_PreservesExistingNumericValues()
  {
    string[] expectedNames = """
    Unknown
    Address
    AdminDivision1
    AdminDivision2
    AdminDivision3
    AdministrativeBuilding
    AdministrativeDivision
    AgriculturalStructure
    Airport
    AirportRunway
    AmusementPark
    AncientSite
    Aquarium
    Archipelago
    Autorail
    Basin
    Battlefield
    Bay
    Beach
    BorderPost
    Bridge
    BusinessCategory
    BusinessCenter
    BusinessName
    BusinessStructure
    BusStation
    Camp
    Canal
    Cave
    CelestialFeature
    Cemetery
    Census1
    Census2
    CensusDistrict
    Channel
    Church
    CityHall
    Cliff
    ClimateRegion
    Coast
    CommunityCenter
    Continent
    ConventionCenter
    CountryRegion
    Courthouse
    Crater
    CulturalRegion
    Current
    Dam
    Delta
    Dependent
    Desert
    DisputedArea
    DrainageBasin
    Dune
    EarthquakeEpicenter
    Ecoregion
    EducationalStructure
    ElevationZone
    Factory
    FerryRoute
    FerryTerminal
    FishHatchery
    Forest
    FormerAdministrativeDivision
    FormerPoliticalUnit
    FormerSovereign
    Fort
    Garden
    GeodeticFeature
    GeoEntity
    GeographicPole
    Geyser
    Glacier
    GolfCourse
    GovernmentStructure
    Heliport
    Hemisphere
    HigherEducationFacility
    HistoricalSite
    Hospital
    HotSpring
    Ice
    IndigenousPeoplesReserve
    IndustrialStructure
    InformationCenter
    InternationalDateline
    InternationalOrganization
    Island
    Isthmus
    Junction
    Lake
    LandArea
    Landform
    LandmarkBuilding
    LatitudeLine
    Library
    Lighthouse
    LinguisticRegion
    LongitudeLine
    MagneticPole
    Marina
    Market
    MedicalStructure
    MetroStation
    MilitaryBase
    Mine
    Mission
    Monument
    Mosque
    Mountain
    MountainRange
    Museum
    NauticalStructure
    NavigationalStructure
    Neighborhood
    Oasis
    ObservationPoint
    Ocean
    OfficeBuilding
    Park
    ParkAndRide
    Pass
    Peninsula
    Plain
    Planet
    Plate
    Plateau
    PlayingField
    Pole
    PoliceStation
    PoliticalUnit
    PopulatedPlace
    Postcode
    Postcode1
    Postcode2
    Postcode3
    Postcode4
    PostOffice
    PowerStation
    Prison
    Promontory
    RaceTrack
    Railway
    RailwayStation
    RecreationalStructure
    Reef
    Region
    ReligiousRegion
    ReligiousStructure
    ResearchStructure
    Reserve
    ResidentialStructure
    RestArea
    River
    Road
    RoadBlock
    RoadIntersection
    Ruin
    Satellite
    School
    ScientificResearchBase
    Sea
    SeaplaneLandingArea
    ShipWreck
    ShoppingCenter
    Shrine
    Site
    SkiArea
    Sovereign
    SpotElevation
    Spring
    Stadium
    StatisticalDistrict
    Structure
    TectonicBoundary
    TectonicFeature
    Temple
    TimeZone
    TouristStructure
    Trail
    TransportationStructure
    Tunnel
    UnderwaterFeature
    UrbanRegion
    Valley
    Volcano
    Wall
    Waterfall
    WaterFeature
    Well
    Wetland
    Zoo
    PointOfInterest
    """.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    Assert.Equal(expectedNames.Length, Enum.GetNames<EntityType>().Length);

    for (int index = 0; index < expectedNames.Length; index++)
    {
      var entityType = Enum.Parse<EntityType>(expectedNames[index]);
      var expectedValue = index == 0 ? -1 : index - 1;
      Assert.Equal(expectedValue, (int)entityType);
    }
  }

    [Fact]
    public void Response_WithLocationResource_DeserializesToLocation()
    {
        // Arrange
        const string json = """
        {
          "resourceSets": [
            {
              "resources": [
                {
                  "name": "White House",
                  "entityType": "Address",
                  "confidence": "High",
                  "point": { "type": "Point", "coordinates": [38.8976777, -77.036517] },
                  "address": {
                    "formattedAddress": "1600 Pennsylvania Ave NW, Washington, DC 20500",
                    "addressLine": "1600 Pennsylvania Ave NW",
                    "adminDistrict": "DC",
                    "adminDistrict2": "District of Columbia",
                    "countryRegion": "United States",
                    "locality": "Washington",
                    "postalCode": "20500"
                  }
                }
              ]
            }
          ]
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<Response>(json, JsonExtensions.JsonOptions);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response!.ResourceSets);
        Assert.Single(response.ResourceSets[0].Resources);
        Assert.IsType<Geocoding.Microsoft.Json.Location>(response.ResourceSets[0].Resources[0]);
        Assert.Equal("White House", response.ResourceSets[0].Resources[0].Name);
        Assert.NotNull(response.ResourceSets[0].Resources[0].Point);
        Assert.Equal(38.8976777, response.ResourceSets[0].Resources[0].Point!.Coordinates[0]);
        Assert.Single(response.ResourceSets[0].Locations);
        Assert.Equal("White House", response.ResourceSets[0].Locations[0].Name);
    }

    [Fact]
    public void Response_WithRouteResource_DeserializesToRoute()
    {
        // Arrange
        const string json = """
        {
          "resourceSets": [
            {
              "resources": [
                {
                  "name": "Route",
                  "distanceUnit": "Kilometer",
                  "durationUnit": "Second",
                  "travelDistance": 1.2,
                  "travelDuration": 120,
                  "routeLegs": [],
                  "routePath": { "line": { "point": [] } }
                }
              ]
            }
          ]
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<Response>(json, JsonExtensions.JsonOptions);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response!.ResourceSets);
        Assert.Single(response.ResourceSets[0].Resources);
        Assert.IsType<Geocoding.Microsoft.Json.Route>(response.ResourceSets[0].Resources[0]);
        Assert.Empty(response.ResourceSets[0].Locations);
    }
}
