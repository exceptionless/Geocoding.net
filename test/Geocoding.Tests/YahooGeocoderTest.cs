#pragma warning disable CS0618
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

    [Theory(Skip = "oauth not working for yahoo - see issue #27")]
    [MemberData(nameof(AddressData), MemberType = typeof(GeocoderTest))]
    public override Task Geocode_ValidAddress_ReturnsExpectedResult(string address)
    {
        return base.Geocode_ValidAddress_ReturnsExpectedResult(address);
    }

    [Fact(Skip = "oauth not working for yahoo - see issue #27")]
    public override Task Geocode_NormalizedAddress_ReturnsExpectedResult()
    {
        return base.Geocode_NormalizedAddress_ReturnsExpectedResult();
    }

    [Theory(Skip = "oauth not working for yahoo - see issue #27")]
    [MemberData(nameof(CultureData), MemberType = typeof(GeocoderTest))]
    public override Task Geocode_DifferentCulture_ReturnsExpectedResult(string cultureName)
    {
        return base.Geocode_DifferentCulture_ReturnsExpectedResult(cultureName);
    }

    [Theory(Skip = "oauth not working for yahoo - see issue #27")]
    [MemberData(nameof(CultureData), MemberType = typeof(GeocoderTest))]
    public override Task ReverseGeocode_DifferentCulture_ReturnsExpectedResult(string cultureName)
    {
        return base.ReverseGeocode_DifferentCulture_ReturnsExpectedResult(cultureName);
    }

    [Fact(Skip = "oauth not working for yahoo - see issue #27")]
    public override Task Geocode_InvalidAddress_ReturnsEmpty()
    {
        return base.Geocode_InvalidAddress_ReturnsEmpty();
    }

    [Theory(Skip = "oauth not working for yahoo - see issue #27")]
    [MemberData(nameof(SpecialCharacterAddressData), MemberType = typeof(GeocoderTest))]
    public override Task Geocode_SpecialCharacters_ReturnsResults(string address)
    {
        return base.Geocode_SpecialCharacters_ReturnsResults(address);
    }

    [Theory(Skip = "oauth not working for yahoo - see issue #27")]
    [MemberData(nameof(StreetIntersectionAddressData), MemberType = typeof(GeocoderTest))]
    public override Task Geocode_StreetIntersection_ReturnsResults(string address)
    {
        return base.Geocode_StreetIntersection_ReturnsResults(address);
    }

    [Fact(Skip = "oauth not working for yahoo - see issue #27")]
    public override Task ReverseGeocode_WhiteHouseCoordinates_ReturnsExpectedArea()
    {
        return base.ReverseGeocode_WhiteHouseCoordinates_ReturnsExpectedArea();
    }

    [Theory(Skip = "oauth not working for yahoo - see issue #27")]
    [MemberData(nameof(InvalidZipCodeAddressData), MemberType = typeof(GeocoderTest))]
    public override Task Geocode_InvalidZipCode_ReturnsResults(string address)
    {
        return base.Geocode_InvalidZipCode_ReturnsResults(address);
    }
}
#pragma warning restore CS0618
