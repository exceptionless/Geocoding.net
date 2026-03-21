using Geocoding.Here;
using Geocoding.MapQuest;
using Geocoding.Microsoft;
using System.Reflection;
using Xunit;

namespace Geocoding.Tests;

public class ProviderCompatibilityTest
{
    [Fact]
    public void AzureMapsGeocoder_EmptyApiKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new AzureMapsGeocoder(String.Empty));
    }

    [Fact]
    public void HereGeocoder_LegacyAppIdAppCode_ThrowsNotSupportedException()
    {
        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(() => new HereGeocoder("legacy-app-id", "legacy-app-code"));
        Assert.Contains("API key", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MapQuestGeocoder_SetUseOSM_ThrowsNotSupportedException()
    {
        // Arrange
        var geocoder = new MapQuestGeocoder("mapquest-key");

        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(() => geocoder.UseOSM = true);
        Assert.Contains("no longer supported", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BingMapsGeocoder_EmptyApiKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new BingMapsGeocoder(String.Empty));
    }

    [Fact]
    public void BuildSearchUri_WithConfiguredBias_IncludesAllParameters()
    {
        // Arrange
        var geocoder = new AzureMapsGeocoder("azure-key")
        {
            Culture = "fr",
            MaxResults = 5,
            UserLocation = new Location(47.6101, -122.2015),
            UserMapView = new Bounds(47.5, -122.4, 47.8, -122.1)
        };

        // Act
        var method = typeof(AzureMapsGeocoder).GetMethod("BuildSearchUri", BindingFlags.Instance | BindingFlags.NonPublic);
        var uri = (Uri)method!.Invoke(geocoder, new object[] { "1600 Pennsylvania Ave NW, Washington, DC" })!;
        var value = uri.ToString();

        // Assert
        Assert.Contains("subscription-key=azure-key", value, StringComparison.Ordinal);
        Assert.Contains("language=fr", value, StringComparison.Ordinal);
        Assert.Contains("limit=5", value, StringComparison.Ordinal);
        Assert.Contains("lat=47.6101", value, StringComparison.Ordinal);
        Assert.Contains("lon=-122.2015", value, StringComparison.Ordinal);
        Assert.Contains("topLeft=47.8%2C-122.4", value, StringComparison.Ordinal);
        Assert.Contains("btmRight=47.5%2C-122.1", value, StringComparison.Ordinal);
    }

    [Fact]
    public void ParseResponse_PointOfInterest_ReturnsCorrectTypeAndNeighborhood()
    {
        // Arrange
        var geocoder = new AzureMapsGeocoder("azure-key");
        var response = CreateAzureSearchResponse("POI", "AddressPoint", null, "Capitol Hill");

        // Act & Assert (without neighborhood)
        var withoutNeighborhood = InvokeAzureParseResponse(geocoder, response);
        var parsedWithoutNeighborhood = Assert.Single(withoutNeighborhood);
        Assert.Equal(EntityType.PointOfInterest, parsedWithoutNeighborhood.Type);
        Assert.Equal(ConfidenceLevel.High, parsedWithoutNeighborhood.Confidence);
        Assert.Equal(String.Empty, parsedWithoutNeighborhood.Neighborhood);

        // Act & Assert (with neighborhood)
        geocoder.IncludeNeighborhood = true;
        var withNeighborhood = InvokeAzureParseResponse(geocoder, response);
        var parsedWithNeighborhood = Assert.Single(withNeighborhood);
        Assert.Equal("Capitol Hill", parsedWithNeighborhood.Neighborhood);
    }

    private static AzureMapsAddress[] InvokeAzureParseResponse(AzureMapsGeocoder geocoder, object response)
    {
        var method = typeof(AzureMapsGeocoder).GetMethod("ParseResponse", BindingFlags.Instance | BindingFlags.NonPublic);
        return ((IEnumerable<AzureMapsAddress>)method!.Invoke(geocoder, new[] { response })!).ToArray();
    }

    private static object CreateAzureSearchResponse(string resultType, string matchType, string? entityType, string municipalitySubdivision)
    {
        var geocoderType = typeof(AzureMapsGeocoder);
        var responseType = geocoderType.GetNestedType("AzureSearchResponse", BindingFlags.NonPublic)!;
        var resultPayloadType = geocoderType.GetNestedType("AzureSearchResult", BindingFlags.NonPublic)!;
        var addressType = geocoderType.GetNestedType("AzureAddressPayload", BindingFlags.NonPublic)!;
        var positionType = geocoderType.GetNestedType("AzurePosition", BindingFlags.NonPublic)!;
        var poiType = geocoderType.GetNestedType("AzurePointOfInterest", BindingFlags.NonPublic)!;

        var response = Activator.CreateInstance(responseType, true)!;
        var result = Activator.CreateInstance(resultPayloadType, true)!;
        var address = Activator.CreateInstance(addressType, true)!;
        var position = Activator.CreateInstance(positionType, true)!;
        var poi = Activator.CreateInstance(poiType, true)!;

        SetProperty(result, "Type", resultType);
        SetProperty(result, "MatchType", matchType);
        SetProperty(result, "EntityType", entityType);
        SetProperty(result, "Address", address);
        SetProperty(result, "Position", position);
        SetProperty(result, "Poi", poi);

        SetProperty(address, "FreeformAddress", "1 Main St, Seattle, WA 98101, United States");
        SetProperty(address, "StreetNumber", "1");
        SetProperty(address, "StreetName", "Main St");
        SetProperty(address, "Municipality", "Seattle");
        SetProperty(address, "MunicipalitySubdivision", municipalitySubdivision);
        SetProperty(address, "CountrySubdivisionName", "Washington");
        SetProperty(address, "CountrySecondarySubdivision", "King");
        SetProperty(address, "PostalCode", "98101");
        SetProperty(address, "Country", "United States");

        SetProperty(position, "Lat", 47.6101d);
        SetProperty(position, "Lon", -122.2015d);

        SetProperty(poi, "Name", "Example POI");

        var results = Array.CreateInstance(resultPayloadType, 1);
        results.SetValue(result, 0);
        SetProperty(response, "Results", results);

        return response;
    }

    private static void SetProperty(object instance, string propertyName, object? value)
    {
        instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(instance, value);
    }
}
