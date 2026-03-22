using System.Net;
using System.Net.Http;
using Geocoding.Microsoft;
using Xunit;
using MicrosoftJson = Geocoding.Microsoft.Json;

namespace Geocoding.Tests;

[Collection("Settings")]
public class BingMapsTest : GeocoderTest
{
    public BingMapsTest(SettingsFixture settings)
        : base(settings) { }

    protected override IGeocoder CreateGeocoder()
    {
        SettingsFixture.SkipIfMissing(_settings.BingMapsKey, nameof(SettingsFixture.BingMapsKey));
        return new BingMapsGeocoder(_settings.BingMapsKey);
    }

    [Theory]
    [InlineData("United States", "fr", "États-Unis")]
    [InlineData("Montreal", "en", "Montreal, QC")]
    [InlineData("Montreal", "fr", "Montréal, QC")]
    public async Task Geocode_WithCulture_ReturnsLocalizedAddress(string address, string culture, string result)
    {
        // Arrange
        var geocoder = GetGeocoder<BingMapsGeocoder>();
        geocoder.Culture = culture;

        // Act
        var addresses = (await geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.Equal(result, addresses[0].FormattedAddress);
    }

    [Theory]
    [InlineData("Montreal", 45.512401580810547, -73.554679870605469, "Canada")]
    [InlineData("Montreal", 43.949058532714844, 0.20011000335216522, "France")]
    [InlineData("Montreal", 46.428329467773438, -90.241783142089844, "United States")]
    public async Task Geocode_WithUserLocation_ReturnsBiasedResult(string address, double userLatitude, double userLongitude, string country)
    {
        // Arrange
        var geocoder = GetGeocoder<BingMapsGeocoder>();
        geocoder.UserLocation = new Location(userLatitude, userLongitude);

        // Act
        var addresses = (await geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.Contains(addresses, x => String.Equals(x.CountryRegion, country, StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("Montreal", 45, -73, 46, -74, "Canada")]
    [InlineData("Montreal", 43, 0, 44, 1, "France")]
    [InlineData("Montreal", 46, -90, 47, -91, "United States")]
    public async Task Geocode_WithUserMapView_ReturnsBiasedResult(string address, double userLatitude1, double userLongitude1, double userLatitude2, double userLongitude2, string country)
    {
        // Arrange
        var geocoder = GetGeocoder<BingMapsGeocoder>();
        geocoder.UserMapView = new Bounds(userLatitude1, userLongitude1, userLatitude2, userLongitude2);
        geocoder.MaxResults = 20;

        // Act
        var addresses = (await geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.Contains(addresses, x => String.Equals(x.CountryRegion, country, StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("24 sussex drive ottawa, ontario")]
    public async Task Geocode_WithIncludeNeighborhood_ReturnsNeighborhood(string address)
    {
        // Arrange
        var geocoder = GetGeocoder<BingMapsGeocoder>();
        geocoder.IncludeNeighborhood = true;

        // Act
        var addresses = (await geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.NotNull(addresses[0].Neighborhood);
    }

    [Fact]
    //https://github.com/chadly/Geocoding.net/issues/8
    public async Task ReverseGeocode_WhiteHouseCoordinates_ReturnsResults()
    {
        var geocoder = GetGeocoder<BingMapsGeocoder>();

        // Act
        var addresses = (await geocoder.ReverseGeocodeAsync(38.8976777, -77.036517, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.NotEmpty(addresses);
    }

    [Fact]
    public void Constructor_EmptyApiKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new BingMapsGeocoder(String.Empty));
    }

    [Fact]
    public void ParseResponse_EmptyResourceSets_ReturnsEmpty()
    {
        // Arrange
        var geocoder = new TestableBingMapsGeocoder();
        var response = new MicrosoftJson.Response { ResourceSets = Array.Empty<MicrosoftJson.ResourceSet>() };

        // Act
        var addresses = geocoder.Parse(response).ToArray();

        // Assert
        Assert.Empty(addresses);
    }

    [Fact]
    public void ParseResponse_LocationWithShortCoordinates_SkipsEntry()
    {
        // Arrange
        var geocoder = new TestableBingMapsGeocoder();
        var response = new MicrosoftJson.Response
        {
            ResourceSets =
            [
                new MicrosoftJson.ResourceSet
                {
                    Resources =
                    [
                        new MicrosoftJson.Location
                        {
                            Point = new MicrosoftJson.Point { Coordinates = [38.8976777] },
                            Address = new MicrosoftJson.Address { FormattedAddress = "White House" },
                            EntityType = nameof(EntityType.Address),
                            Confidence = "High"
                        }
                    ]
                }
            ]
        };

        // Act
        var addresses = geocoder.Parse(response).ToArray();

        // Assert
        Assert.Empty(addresses);
    }

    [Fact]
    public void ParseResponse_LocationWithNullCoordinates_SkipsEntry()
    {
        // Arrange
        var geocoder = new TestableBingMapsGeocoder();
        var response = new MicrosoftJson.Response
        {
            ResourceSets =
            [
                new MicrosoftJson.ResourceSet
                {
                    Resources =
                    [
                        new MicrosoftJson.Location
                        {
                            Point = new MicrosoftJson.Point { Coordinates = null! },
                            Address = new MicrosoftJson.Address { FormattedAddress = "White House" },
                            EntityType = nameof(EntityType.Address),
                            Confidence = "High"
                        }
                    ]
                }
            ]
        };

        // Act
        var addresses = geocoder.Parse(response).ToArray();

        // Assert
        Assert.Empty(addresses);
    }

    [Fact]
    public void ParseResponse_LocationWithBlankFormattedAddress_SkipsEntry()
    {
        // Arrange
        var geocoder = new TestableBingMapsGeocoder();
        var response = new MicrosoftJson.Response
        {
            ResourceSets =
            [
                new MicrosoftJson.ResourceSet
                {
                    Resources =
                    [
                        new MicrosoftJson.Location
                        {
                            Point = new MicrosoftJson.Point { Coordinates = [38.8976777, -77.036517] },
                            Address = new MicrosoftJson.Address { FormattedAddress = "   " },
                            EntityType = nameof(EntityType.Address),
                            Confidence = "High"
                        }
                    ]
                }
            ]
        };

        // Act
        var addresses = geocoder.Parse(response).ToArray();

        // Assert
        Assert.Empty(addresses);
    }

    [Fact]
    public async Task Geocode_HttpFailure_PreservesInnerExceptionPreview()
    {
        // Arrange
        var body = new string('x', 300);
        var geocoder = new TestableBingMapsGeocoder(new TestHttpMessageHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            ReasonPhrase = "Bad Request",
            Content = new StringContent(body)
        })));

        // Act
        var exception = await Assert.ThrowsAsync<BingGeocodingException>(() => geocoder.GeocodeAsync("1600 pennsylvania ave nw, washington dc", TestContext.Current.CancellationToken));

        // Assert
        Assert.NotNull(exception.InnerException);
        Assert.Contains("Bing Maps request failed (400 Bad Request).", exception.InnerException!.Message, StringComparison.Ordinal);
        Assert.Contains("Response preview:", exception.InnerException.Message, StringComparison.Ordinal);
        Assert.DoesNotContain(body, exception.InnerException.Message, StringComparison.Ordinal);
    }

    private sealed class TestableBingMapsGeocoder : BingMapsGeocoder
    {
        private readonly HttpMessageHandler? _handler;

        public TestableBingMapsGeocoder() : base("bing-key") { }

        public TestableBingMapsGeocoder(HttpMessageHandler handler)
            : base("bing-key")
        {
            _handler = handler;
        }

        protected override HttpClient BuildClient()
        {
            return _handler is null ? base.BuildClient() : new HttpClient(_handler, disposeHandler: false);
        }

        public IEnumerable<BingAddress> Parse(MicrosoftJson.Response response)
        {
            return ParseResponse(response);
        }
    }
}
