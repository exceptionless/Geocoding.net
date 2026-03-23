using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using Geocoding.Extensions;
using Geocoding.MapQuest;
using Geocoding.Tests.Utility;
using Xunit;

namespace Geocoding.Tests;

[Collection("Settings")]
public class MapQuestGeocoderTest : GeocoderTest
{
    public MapQuestGeocoderTest(SettingsFixture settings)
        : base(settings) { }

    protected override IGeocoder CreateGeocoder()
    {
        SettingsFixture.SkipIfMissing(_settings.MapQuestKey, nameof(SettingsFixture.MapQuestKey));
        return new MapQuestGeocoder(_settings.MapQuestKey)
        {
            UseOSM = false
        };
    }

    // Regression test: Addresses with Quality=NEIGHBORHOOD are not returned
    [Fact]
    public virtual async Task Geocode_NeighborhoodAddress_ReturnsResults()
    {
        var geocoder = GetGeocoder<MapQuestGeocoder>();

        // Act
        var addresses = (await geocoder.GeocodeAsync("North Sydney, New South Wales, Australia", TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.NotEmpty(addresses);
    }

    [Fact]
    public void UseOSM_SetTrue_ThrowsNotSupportedException()
    {
        // Arrange
        var geocoder = new MapQuestGeocoder("mapquest-key");

        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(() => geocoder.UseOSM = true);
        Assert.Contains("no longer supported", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RequestVerb_Normalization_IsCultureInvariant()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUICulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
            CultureInfo.CurrentUICulture = new CultureInfo("tr-TR");
            var request = new TestRequest("mapquest-key");

            // Act
            request.SetVerb("mixid");

            // Assert
            Assert.Equal("MIXID", request.RequestVerb);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUICulture;
        }
    }

    [Fact]
    public void MapQuestLocation_Deserialization_PreservesProviderDefaults()
    {
        // Arrange
        const string json = """
            {
              "location": "1600 Pennsylvania Ave NW, Washington, DC 20500, US",
              "latLng": { "lat": 38.8977, "lng": -77.0365 },
              "displayLatLng": { "lat": 38.8977, "lng": -77.0365 },
              "street": "1600 Pennsylvania Ave NW",
              "adminArea5": "Washington",
              "adminArea3": "DC",
              "adminArea1": "US",
              "postalCode": "20500"
            }
            """;

        // Act
        var location = JsonSerializer.Deserialize<MapQuestLocation>(json, JsonExtensions.JsonOptions);

        // Assert
        Assert.NotNull(location);
        Assert.Equal("MapQuest", location.Provider);
        Assert.Equal("1600 Pennsylvania Ave NW, Washington, DC 20500, US", location.FormattedAddress);
        Assert.Equal(new Location(38.8977, -77.0365), location.Coordinates);
    }

    [Fact]
    public async Task CreateRequest_GeocodeRequest_CreatesJsonPost()
    {
        // Arrange
        var geocoder = new MapQuestGeocoder("mapquest-key");
        var requestData = new GeocodeRequest("mapquest-key", "1600 pennsylvania ave nw, washington dc");
        var createRequest = typeof(MapQuestGeocoder).GetMethod("CreateRequest", BindingFlags.Instance | BindingFlags.NonPublic)!;

        // Act
        using var request = (HttpRequestMessage)createRequest.Invoke(geocoder, [requestData])!;
        var body = await request.Content!.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal(requestData.RequestUri, request.RequestUri);
        Assert.Equal("application/json; charset=utf-8", request.Content.Headers.ContentType!.ToString());
        Assert.Contains("1600 pennsylvania ave", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Geocode_ConnectionFailure_IncludesRequestContext()
    {
        // Arrange
        var geocoder = new TestableMapQuestGeocoder(new TestHttpMessageHandler((_, _) => throw new HttpRequestException("Name or service not known")));

        // Act
        var exception = await Assert.ThrowsAsync<MapQuestGeocodingException>(() => geocoder.GeocodeAsync("1600 pennsylvania ave nw, washington dc", TestContext.Current.CancellationToken));

        // Assert
        Assert.Contains("[POST]", exception.Message, StringComparison.Ordinal);
        Assert.Contains("mapquestapi.com/geocoding/v1/address", exception.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("key=mapquest-key", exception.Message, StringComparison.Ordinal);
        Assert.IsType<HttpRequestException>(exception.InnerException);
    }

    [Fact]
    public async Task Geocode_StatusFailure_UsesTrimmedPreviewMessage()
    {
        // Arrange
        var body = new string('x', 300);
        var geocoder = new TestableMapQuestGeocoder(new TestHttpMessageHandler((_, _) => TestHttpMessageHandler.CreateResponseAsync(HttpStatusCode.BadGateway, "Bad Gateway", body)));

        // Act
        var exception = await Assert.ThrowsAsync<MapQuestGeocodingException>(() => geocoder.GeocodeAsync("1600 pennsylvania ave nw, washington dc", TestContext.Current.CancellationToken));

        // Assert
        Assert.Contains("502", exception.Message, StringComparison.Ordinal);
        Assert.Contains("Response preview:", exception.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("key=mapquest-key", exception.Message, StringComparison.Ordinal);
        Assert.DoesNotContain(body, exception.Message, StringComparison.Ordinal);
    }

    private sealed class TestableMapQuestGeocoder : MapQuestGeocoder
    {
        private readonly HttpMessageHandler _handler;

        public TestableMapQuestGeocoder(HttpMessageHandler handler)
            : base("mapquest-key")
        {
            _handler = handler;
        }

        protected override HttpClient BuildClient()
        {
            return new HttpClient(_handler, disposeHandler: false);
        }
    }

    private sealed class TestRequest : BaseRequest
    {
        public TestRequest(string key)
            : base(key) { }

        public override string RequestAction => "address";

        public void SetVerb(string verb)
        {
            RequestVerb = verb;
        }
    }
}
