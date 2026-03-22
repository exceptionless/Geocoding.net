using System.Collections;
using System.Reflection;
using Geocoding.Here;
using Xunit;

namespace Geocoding.Tests;

[Collection("Settings")]
public class HereAsyncGeocoderTest : AsyncGeocoderTest
{
    public HereAsyncGeocoderTest(SettingsFixture settings)
        : base(settings) { }

    protected override IGeocoder CreateAsyncGeocoder()
    {
        if (!String.IsNullOrWhiteSpace(_settings.HereAppId) && !String.IsNullOrWhiteSpace(_settings.HereAppCode))
            return new HereGeocoder(_settings.HereAppId, _settings.HereAppCode);

        if (!String.IsNullOrWhiteSpace(_settings.HereApiKey))
            return new HereGeocoder(_settings.HereApiKey);

        if (!String.IsNullOrWhiteSpace(_settings.HereAppId))
            SettingsFixture.SkipIfMissing(_settings.HereAppCode, nameof(SettingsFixture.HereAppCode));

        if (!String.IsNullOrWhiteSpace(_settings.HereAppCode))
            SettingsFixture.SkipIfMissing(_settings.HereAppId, nameof(SettingsFixture.HereAppId));

        SettingsFixture.SkipIfMissing(_settings.HereApiKey, nameof(SettingsFixture.HereApiKey));
        throw new InvalidOperationException("HERE test credentials are unavailable.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public Task Geocode_BlankAddress_ThrowsArgumentException(string address)
    {
        // Arrange
        var geocoder = new HereGeocoder("here-api-key");

        // Act & Assert
        return Assert.ThrowsAsync<ArgumentException>(() => geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken));
    }

    [Fact]
    public void Constructor_LegacyAppIdAppCode_DoesNotThrow()
    {
        // Act
        var geocoder = new HereGeocoder("legacy-app-id", "legacy-app-code");

        // Assert
        Assert.NotNull(geocoder);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_BlankLegacyAppCode_ThrowsArgumentException(string appCode)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new HereGeocoder("legacy-app-id", appCode));
    }

    [Fact]
    public void LegacyConstructor_QueryUrl_UsesLegacyEndpoint()
    {
        // Arrange
        var geocoder = new HereGeocoder("legacy/app-id", "legacy+app-code")
        {
            UserLocation = new Location(45.5017, -73.5673)
        };

        // Act
        var queryUrl = (Uri)typeof(HereGeocoder)
            .GetMethod("GetQueryUrl", BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(string)], null)!
            .Invoke(geocoder, ["1600 pennsylvania ave nw, washington dc"])!;

        // Assert
        Assert.Contains("geocoder.api.here.com/6.2/geocode.json", queryUrl.AbsoluteUri, StringComparison.Ordinal);
        Assert.Contains("app_id=legacy%2Fapp-id", queryUrl.AbsoluteUri, StringComparison.Ordinal);
        Assert.Contains("app_code=legacy%2Bapp-code", queryUrl.AbsoluteUri, StringComparison.Ordinal);
        Assert.Contains("prox=45.5017%2C-73.5673", queryUrl.AbsoluteUri, StringComparison.Ordinal);
        Assert.Contains("searchtext=1600", queryUrl.AbsoluteUri, StringComparison.Ordinal);
    }

    [Fact]
    public void LegacyConstructor_BlankStructuredAddress_ThrowsArgumentException()
    {
        // Arrange
        var geocoder = new HereGeocoder("legacy-app-id", "legacy-app-code");

        // Act & Assert
        Assert.Throws<TargetInvocationException>(() => typeof(HereGeocoder)
            .GetMethod("GetQueryUrl", BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(string), typeof(string), typeof(string), typeof(string), typeof(string)], null)!
            .Invoke(geocoder, [String.Empty, String.Empty, String.Empty, String.Empty, String.Empty]));
    }

    [Fact]
    public void LegacyConstructor_ParseLegacyResponse_MapsAddress()
    {
        // Arrange
        var geocoder = new HereGeocoder("legacy-app-id", "legacy-app-code");
        var assembly = typeof(HereGeocoder).Assembly;
        var responseType = assembly.GetType("Geocoding.Here.Json.Response")!;
        var viewType = assembly.GetType("Geocoding.Here.Json.View")!;
        var resultType = assembly.GetType("Geocoding.Here.Json.Result")!;
        var locationType = assembly.GetType("Geocoding.Here.Json.Location")!;
        var coordinateType = assembly.GetType("Geocoding.Here.Json.GeoCoordinate")!;
        var addressType = assembly.GetType("Geocoding.Here.Json.Address")!;

        var response = Activator.CreateInstance(responseType)!;
        var view = Activator.CreateInstance(viewType)!;
        var result = Activator.CreateInstance(resultType)!;
        var location = Activator.CreateInstance(locationType)!;
        var coordinate = Activator.CreateInstance(coordinateType)!;
        var address = Activator.CreateInstance(addressType)!;

        coordinateType.GetProperty("Latitude")!.SetValue(coordinate, 38.8976777);
        coordinateType.GetProperty("Longitude")!.SetValue(coordinate, -77.036517);

        addressType.GetProperty("Label")!.SetValue(address, "1600 Pennsylvania Avenue NW, Washington, DC 20500, United States");
        addressType.GetProperty("Street")!.SetValue(address, "Pennsylvania Avenue NW");
        addressType.GetProperty("HouseNumber")!.SetValue(address, "1600");
        addressType.GetProperty("City")!.SetValue(address, "Washington");
        addressType.GetProperty("State")!.SetValue(address, "DC");
        addressType.GetProperty("PostalCode")!.SetValue(address, "20500");
        addressType.GetProperty("Country")!.SetValue(address, "United States");

        locationType.GetProperty("LocationType")!.SetValue(location, nameof(HereLocationType.Address));
        locationType.GetProperty("DisplayPosition")!.SetValue(location, coordinate);
        locationType.GetProperty("Address")!.SetValue(location, address);

        resultType.GetProperty("Location")!.SetValue(result, location);

        var resultsArray = Array.CreateInstance(resultType, 1);
        resultsArray.SetValue(result, 0);
        viewType.GetProperty("Result")!.SetValue(view, resultsArray);

        var viewsArray = Array.CreateInstance(viewType, 1);
        viewsArray.SetValue(view, 0);
        responseType.GetProperty("View")!.SetValue(response, viewsArray);

        // Act
        var sequence = (IEnumerable)typeof(HereGeocoder)
            .GetMethod("ParseLegacyResponse", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(geocoder, [response])!;
        var addresses = sequence.Cast<HereAddress>().ToArray();

        // Assert
        var parsed = Assert.Single(addresses);
        Assert.Equal("1600 Pennsylvania Avenue NW, Washington, DC 20500, United States", parsed.FormattedAddress);
        Assert.Equal(HereLocationType.Address, parsed.Type);
        Assert.Equal(38.8976777, parsed.Coordinates.Latitude, 6);
        Assert.Equal(-77.036517, parsed.Coordinates.Longitude, 6);
    }

    [Fact]
    public void LegacyConstructor_ReverseQueryUrl_DoesNotDuplicateUserLocationBias()
    {
        // Arrange
        var geocoder = new HereGeocoder("legacy-app-id", "legacy-app-code")
        {
            UserLocation = new Location(45.5017, -73.5673)
        };

        // Act
        var queryUrl = (Uri)typeof(HereGeocoder)
            .GetMethod("GetQueryUrl", BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(double), typeof(double)], null)!
            .Invoke(geocoder, [38.8976777, -77.036517])!;

        // Assert
        Assert.Contains("prox=38.8976777%2C-77.036517", queryUrl.AbsoluteUri, StringComparison.Ordinal);
        Assert.DoesNotContain("45.5017", queryUrl.AbsoluteUri, StringComparison.Ordinal);
    }

    [Fact]
    public void LegacyConstructor_ParseLegacyResponse_WithoutDisplayPosition_SkipsEntry()
    {
        // Arrange
        var geocoder = new HereGeocoder("legacy-app-id", "legacy-app-code");
        var assembly = typeof(HereGeocoder).Assembly;
        var responseType = assembly.GetType("Geocoding.Here.Json.Response")!;
        var viewType = assembly.GetType("Geocoding.Here.Json.View")!;
        var resultType = assembly.GetType("Geocoding.Here.Json.Result")!;
        var locationType = assembly.GetType("Geocoding.Here.Json.Location")!;

        var response = Activator.CreateInstance(responseType)!;
        var view = Activator.CreateInstance(viewType)!;
        var result = Activator.CreateInstance(resultType)!;
        var location = Activator.CreateInstance(locationType)!;

        locationType.GetProperty("LocationType")!.SetValue(location, nameof(HereLocationType.Address));
        resultType.GetProperty("Location")!.SetValue(result, location);

        var resultsArray = Array.CreateInstance(resultType, 1);
        resultsArray.SetValue(result, 0);
        viewType.GetProperty("Result")!.SetValue(view, resultsArray);

        var viewsArray = Array.CreateInstance(viewType, 1);
        viewsArray.SetValue(view, 0);
        responseType.GetProperty("View")!.SetValue(response, viewsArray);

        // Act
        var sequence = (IEnumerable)typeof(HereGeocoder)
            .GetMethod("ParseLegacyResponse", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(geocoder, [response])!;
        var addresses = sequence.Cast<HereAddress>().ToArray();

        // Assert
        Assert.Empty(addresses);
    }
}
