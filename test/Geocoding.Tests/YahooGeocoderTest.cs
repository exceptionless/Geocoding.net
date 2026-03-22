#pragma warning disable CS0618
using System.Net;
using System.Net.Http;
using System.Reflection;
using Geocoding.Yahoo;
using Xunit;

namespace Geocoding.Tests;

[Collection("Settings")]
public class YahooGeocoderTest : GeocoderTest
{
    public YahooGeocoderTest(SettingsFixture settings)
        : base(settings)
    {
    }

    protected override IGeocoder CreateGeocoder()
    {
        SettingsFixture.SkipIfMissing(_settings.YahooConsumerKey, nameof(SettingsFixture.YahooConsumerKey));
        SettingsFixture.SkipIfMissing(_settings.YahooConsumerSecret, nameof(SettingsFixture.YahooConsumerSecret));
        return new YahooGeocoder(_settings.YahooConsumerKey, _settings.YahooConsumerSecret);
    }

    [Theory]
    [MemberData(nameof(AddressData), MemberType = typeof(GeocoderTest))]
    public override Task Geocode_ValidAddress_ReturnsExpectedResult(string address)
    {
        return base.Geocode_ValidAddress_ReturnsExpectedResult(address);
    }

    [Fact]
    public override Task Geocode_NormalizedAddress_ReturnsExpectedResult()
    {
        return base.Geocode_NormalizedAddress_ReturnsExpectedResult();
    }

    [Theory]
    [MemberData(nameof(CultureData), MemberType = typeof(GeocoderTest))]
    public override Task Geocode_DifferentCulture_ReturnsExpectedResult(string cultureName)
    {
        return base.Geocode_DifferentCulture_ReturnsExpectedResult(cultureName);
    }

    [Theory]
    [MemberData(nameof(CultureData), MemberType = typeof(GeocoderTest))]
    public override Task ReverseGeocode_DifferentCulture_ReturnsExpectedResult(string cultureName)
    {
        return base.ReverseGeocode_DifferentCulture_ReturnsExpectedResult(cultureName);
    }

    [Fact]
    public override Task Geocode_InvalidAddress_ReturnsEmpty()
    {
        return base.Geocode_InvalidAddress_ReturnsEmpty();
    }

    [Theory]
    [MemberData(nameof(SpecialCharacterAddressData), MemberType = typeof(GeocoderTest))]
    public override Task Geocode_SpecialCharacters_ReturnsResults(string address)
    {
        return base.Geocode_SpecialCharacters_ReturnsResults(address);
    }

    [Theory]
    [MemberData(nameof(StreetIntersectionAddressData), MemberType = typeof(GeocoderTest))]
    public override Task Geocode_StreetIntersection_ReturnsResults(string address)
    {
        return base.Geocode_StreetIntersection_ReturnsResults(address);
    }

    [Fact]
    public override Task ReverseGeocode_WhiteHouseCoordinates_ReturnsExpectedArea()
    {
        return base.ReverseGeocode_WhiteHouseCoordinates_ReturnsExpectedArea();
    }

    [Theory]
    [MemberData(nameof(InvalidZipCodeAddressData), MemberType = typeof(GeocoderTest))]
    public override Task Geocode_InvalidZipCode_ReturnsResults(string address)
    {
        return base.Geocode_InvalidZipCode_ReturnsResults(address);
    }

    [Fact]
    public void BuildRequest_GeneratesSignedGetRequest()
    {
        // Arrange
        var geocoder = new YahooGeocoder("consumer-key", "consumer-secret");
        var buildRequest = typeof(YahooGeocoder).GetMethod("BuildRequest", BindingFlags.Instance | BindingFlags.NonPublic)!;

        // Act
        using var request = (HttpRequestMessage)buildRequest.Invoke(geocoder, [YahooGeocoder.ServiceUrl.Replace("{0}", "test")])!;
        var requestUri = request.RequestUri!.ToString();

        // Assert
        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.StartsWith("http://yboss.yahooapis.com/geo/placefinder?", requestUri, StringComparison.Ordinal);
        Assert.Contains("oauth_consumer_key=consumer-key", requestUri, StringComparison.Ordinal);
        Assert.Contains("oauth_nonce=", requestUri, StringComparison.Ordinal);
        Assert.Contains("oauth_signature=", requestUri, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Geocode_StatusFailure_WrapsHttpRequestException()
    {
        // Arrange
        var geocoder = new TestableYahooGeocoder(new TestHttpMessageHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized))));

        // Act
        var exception = await Assert.ThrowsAsync<YahooGeocodingException>(() => geocoder.GeocodeAsync("1600 pennsylvania ave nw, washington dc", TestContext.Current.CancellationToken));

        // Assert
        Assert.IsType<HttpRequestException>(exception.InnerException);
    }

    [Fact]
    public async Task Geocode_TransportFailure_WrapsTransportException()
    {
        // Arrange
        var geocoder = new TestableYahooGeocoder(new TestHttpMessageHandler((_, _) => throw new HttpRequestException("socket failure")));

        // Act
        var exception = await Assert.ThrowsAsync<YahooGeocodingException>(() => geocoder.GeocodeAsync("1600 pennsylvania ave nw, washington dc", TestContext.Current.CancellationToken));

        // Assert
        Assert.IsType<HttpRequestException>(exception.InnerException);
    }

    private sealed class TestableYahooGeocoder : YahooGeocoder
    {
        private readonly HttpMessageHandler _handler;

        public TestableYahooGeocoder(HttpMessageHandler handler)
            : base("consumer-key", "consumer-secret")
        {
            _handler = handler;
        }

        protected override HttpClient BuildClient()
        {
            return new HttpClient(_handler, disposeHandler: false);
        }
    }
}
#pragma warning restore CS0618
