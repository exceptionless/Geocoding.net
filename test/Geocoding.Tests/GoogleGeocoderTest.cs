using System.Net;
using System.Net.Http;
using Geocoding.Google;
using Xunit;

namespace Geocoding.Tests;

[Collection("Settings")]
public class GoogleGeocoderTest : GeocoderTest
{
    public GoogleGeocoderTest(SettingsFixture settings)
        : base(settings) { }

    protected override IGeocoder CreateGeocoder()
    {
        String apiKey = _settings.GoogleApiKey;
        SettingsFixture.SkipIfMissing(apiKey, nameof(SettingsFixture.GoogleApiKey));
        GoogleTestGuard.EnsureAvailable(apiKey);
        return new GoogleGeocoder(apiKey);
    }

    [Theory]
    [InlineData("United States", GoogleAddressType.Country)]
    [InlineData("Illinois, US", GoogleAddressType.AdministrativeAreaLevel1)]
    [InlineData("New York, New York", GoogleAddressType.Locality)]
    [InlineData("90210, US", GoogleAddressType.PostalCode)]
    [InlineData("1600 pennsylvania ave washington dc", GoogleAddressType.Premise)]
    [InlineData("muswellbrook 2 New South Wales Australia", GoogleAddressType.Locality)]
    public async Task Geocode_AddressInput_ReturnsCorrectAddressType(string address, GoogleAddressType type)
    {
        var geocoder = GetGeocoder<GoogleGeocoder>();

        // Act
        var addresses = (await geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.Equal(type, addresses[0].Type);
    }

    [Theory]
    [InlineData("United States", GoogleLocationType.Approximate)]
    [InlineData("Illinois, US", GoogleLocationType.Approximate)]
    [InlineData("Ingalls Corners Road, Canastota, NY 13032, USA", GoogleLocationType.GeometricCenter)]
    [InlineData("51 Harry S. Truman Parkway, Annapolis, MD 21401, USA", GoogleLocationType.RangeInterpolated)]
    [InlineData("1600 pennsylvania ave washington dc", GoogleLocationType.Rooftop)]
    [InlineData("muswellbrook 2 New South Wales Australia", GoogleLocationType.Approximate)]
    public async Task Geocode_AddressInput_ReturnsCorrectLocationType(string address, GoogleLocationType type)
    {
        var geocoder = GetGeocoder<GoogleGeocoder>();

        // Act
        var addresses = (await geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.Equal(type, addresses[0].LocationType);
    }

    [Theory]
    [InlineData("United States", "fr", "États-Unis")]
    [InlineData("Montreal", "en", "Montreal, QC, Canada")]
    [InlineData("Montreal", "fr", "Montréal, QC, Canada")]
    [InlineData("Montreal", "de", "Montreal, Québec, Kanada")]
    public async Task Geocode_WithLanguage_ReturnsLocalizedAddress(string address, string language, string result)
    {
        // Arrange
        var geocoder = GetGeocoder<GoogleGeocoder>();
        geocoder.Language = language;

        // Act
        var addresses = (await geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.StartsWith(result, addresses[0].FormattedAddress);
    }

    [Theory]
    [InlineData("Toledo", "us", "Toledo, OH, USA", null)]
    [InlineData("Toledo", "es", "Toledo, Spain", "Toledo, Toledo, Spain")]
    public async Task Geocode_WithRegionBias_ReturnsBiasedResult(string address, string regionBias, string result1, string? result2)
    {
        // Arrange
        var geocoder = GetGeocoder<GoogleGeocoder>();
        geocoder.RegionBias = regionBias;

        // Act
        var addresses = (await geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        String[] expectedAddresses = String.IsNullOrEmpty(result2) ? new[] { result1 } : new[] { result1, result2 };
        Assert.Contains(addresses[0].FormattedAddress, expectedAddresses);
    }

    [Theory]
    [InlineData("Winnetka", 46, -90, 47, -91, "Winnetka, IL")]
    [InlineData("Winnetka", 34.172684, -118.604794, 34.236144, -118.500938, "Winnetka, Los Angeles, CA")]
    public async Task Geocode_WithBoundsBias_ReturnsBiasedResult(string address, double biasLatitude1, double biasLongitude1, double biasLatitude2, double biasLongitude2, string expectedSubstring)
    {
        // Arrange
        var geocoder = GetGeocoder<GoogleGeocoder>();
        geocoder.BoundsBias = new Bounds(biasLatitude1, biasLongitude1, biasLatitude2, biasLongitude2);

        // Act
        var addresses = (await geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.Contains(expectedSubstring, addresses[0].FormattedAddress);
        Assert.Contains("USA", addresses[0].FormattedAddress, StringComparison.Ordinal);
        Assert.Contains(addresses, x => HasShortName(x, "US"));
    }

    [Theory]
    [InlineData("Wimbledon")]
    [InlineData("Birmingham")]
    [InlineData("Manchester")]
    [InlineData("York")]
    public async Task Geocode_WithGBCountryFilter_ExcludesUSResults(string address)
    {
        // Arrange
        var geocoder = GetGeocoder<GoogleGeocoder>();
        geocoder.ComponentFilters = new List<GoogleComponentFilter>();
        geocoder.ComponentFilters.Add(new GoogleComponentFilter(GoogleComponentFilterType.Country, "GB"));

        // Act
        var addresses = (await geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.DoesNotContain(addresses, x => HasShortName(x, "US"));
        Assert.Contains(addresses, x => HasShortName(x, "GB"));
    }

    [Theory]
    [InlineData("Wimbledon")]
    [InlineData("Birmingham")]
    [InlineData("Manchester")]
    [InlineData("York")]
    public async Task Geocode_WithUSCountryFilter_ExcludesGBResults(string address)
    {
        // Arrange
        var geocoder = GetGeocoder<GoogleGeocoder>();
        geocoder.ComponentFilters = new List<GoogleComponentFilter>();
        geocoder.ComponentFilters.Add(new GoogleComponentFilter(GoogleComponentFilterType.Country, "US"));

        // Act
        var addresses = (await geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.Contains(addresses, x => HasShortName(x, "US"));
        Assert.DoesNotContain(addresses, x => HasShortName(x, "GB"));
    }

    [Theory]
    [InlineData("Washington")]
    [InlineData("Franklin")]
    public async Task Geocode_WithAdminAreaFilter_ReturnsFilteredResults(string address)
    {
        // Arrange
        var geocoder = GetGeocoder<GoogleGeocoder>();
        geocoder.ComponentFilters = new List<GoogleComponentFilter>();
        geocoder.ComponentFilters.Add(new GoogleComponentFilter(GoogleComponentFilterType.AdministrativeArea, "KS"));

        // Act
        var addresses = (await geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.Contains(addresses, x => HasShortName(x, "KS"));
        Assert.DoesNotContain(addresses, x => HasShortName(x, "MA"));
        Assert.DoesNotContain(addresses, x => HasShortName(x, "LA"));
        Assert.DoesNotContain(addresses, x => HasShortName(x, "NJ"));
    }

    [Fact]
    public async Task Geocode_WithPostalCodeFilter_ReturnsResultInExpectedPostalCode()
    {
        // Arrange
        var geocoder = GetGeocoder<GoogleGeocoder>();
        geocoder.ComponentFilters = new List<GoogleComponentFilter>();
        geocoder.ComponentFilters.Add(new GoogleComponentFilter(GoogleComponentFilterType.PostalCode, "94043"));

        // Act
        var addresses = (await geocoder.GeocodeAsync("1600 Amphitheatre Parkway, Mountain View, CA", TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.Contains(addresses, x => HasShortName(x, "94043"));
    }

    [Fact]
    public void GoogleGeocodingException_WithProviderMessage_PreservesStatusAndMessage()
    {
        // Act
        var exception = new GoogleGeocodingException(GoogleStatus.RequestDenied, "This API is not activated on your API project.");

        // Assert
        Assert.Equal(GoogleStatus.RequestDenied, exception.Status);
        Assert.Equal("This API is not activated on your API project.", exception.ProviderMessage);
        Assert.Contains("RequestDenied", exception.Message);
        Assert.Contains("This API is not activated on your API project.", exception.Message);
    }

    [Fact]
    public void GoogleGeocodingException_WithoutProviderMessage_LeavesProviderMessageNull()
    {
        // Act
        var exception = new GoogleGeocodingException(GoogleStatus.OverQueryLimit);

        // Assert
        Assert.Equal(GoogleStatus.OverQueryLimit, exception.Status);
        Assert.Null(exception.ProviderMessage);
        Assert.Contains("OverQueryLimit", exception.Message);
    }

    [Fact]
    public async Task Geocode_HttpFailure_PreservesInnerExceptionPreview()
    {
        // Arrange
        var body = new string('x', 300);
        var geocoder = new TestableGoogleGeocoder(new TestHttpMessageHandler((_, _) => TestHttpMessageHandler.CreateResponseAsync(HttpStatusCode.BadRequest, "Bad Request", body)));

        // Act
        var exception = await Assert.ThrowsAsync<GoogleGeocodingException>(() => geocoder.GeocodeAsync("1600 pennsylvania ave nw, washington dc", TestContext.Current.CancellationToken));

        // Assert
        Assert.NotNull(exception.InnerException);
        Assert.Contains("Google request failed (400 Bad Request).", exception.InnerException!.Message, StringComparison.Ordinal);
        Assert.Contains("Response preview:", exception.InnerException.Message, StringComparison.Ordinal);
        Assert.DoesNotContain(body, exception.InnerException.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Geocode_TransportFailure_WrapsInnerException()
    {
        // Arrange
        var geocoder = new TestableGoogleGeocoder(new TestHttpMessageHandler((_, _) => throw new HttpRequestException("socket failure")));

        // Act
        var exception = await Assert.ThrowsAsync<GoogleGeocodingException>(() => geocoder.GeocodeAsync("1600 pennsylvania ave nw, washington dc", TestContext.Current.CancellationToken));

        // Assert
        Assert.IsType<HttpRequestException>(exception.InnerException);
        Assert.Contains("socket failure", exception.InnerException!.Message, StringComparison.Ordinal);
    }

    private static bool HasShortName(GoogleAddress address, string shortName)
    {
        return address.Components.Any(component => String.Equals(component.ShortName, shortName, StringComparison.Ordinal));
    }

    private sealed class TestableGoogleGeocoder : GoogleGeocoder
    {
        private readonly HttpMessageHandler _handler;

        public TestableGoogleGeocoder(HttpMessageHandler handler)
        {
            _handler = handler;
        }

        protected override HttpClient BuildClient()
        {
            return new HttpClient(_handler, disposeHandler: false);
        }
    }
}
