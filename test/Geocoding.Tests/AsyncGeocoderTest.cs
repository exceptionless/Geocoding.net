using System.Globalization;
using Xunit;

namespace Geocoding.Tests;

public abstract class AsyncGeocoderTest
{
    private readonly IGeocoder _asyncGeocoder;
    protected readonly SettingsFixture _settings;

    protected AsyncGeocoderTest(SettingsFixture settings)
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-us");

        _settings = settings;
        _asyncGeocoder = CreateAsyncGeocoder();
    }

    protected abstract IGeocoder CreateAsyncGeocoder();

    [Fact]
    public async Task Geocode_ValidAddress_ReturnsExpectedResult()
    {
        var addresses = await _asyncGeocoder.GeocodeAsync("1600 pennsylvania ave washington dc", TestContext.Current.CancellationToken);
        addresses.First().AssertWhiteHouse();
    }

    [Fact]
    public async Task Geocode_NormalizedAddress_ReturnsExpectedResult()
    {
        var addresses = await _asyncGeocoder.GeocodeAsync("1600 pennsylvania ave", "washington", "dc", null, null, TestContext.Current.CancellationToken);
        addresses.First().AssertWhiteHouse();
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("cs-CZ")]
    public async Task Geocode_DifferentCulture_ReturnsExpectedResult(string cultureName)
    {
        CultureInfo.CurrentCulture = new CultureInfo(cultureName);

        var addresses = await _asyncGeocoder.GeocodeAsync("24 sussex drive ottawa, ontario", TestContext.Current.CancellationToken);
        addresses.First().AssertCanadianPrimeMinister();
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("cs-CZ")]
    public async Task ReverseGeocode_DifferentCulture_ReturnsExpectedResult(string cultureName)
    {
        CultureInfo.CurrentCulture = new CultureInfo(cultureName);

        var addresses = await _asyncGeocoder.ReverseGeocodeAsync(38.8976777, -77.036517, TestContext.Current.CancellationToken);
        addresses.First().AssertWhiteHouseArea();
    }

    [Fact]
    public async Task Geocode_InvalidAddress_ReturnsEmpty()
    {
        var addresses = await _asyncGeocoder.GeocodeAsync("sdlkf;jasl;kjfldksjfasldf", TestContext.Current.CancellationToken);
        Assert.Empty(addresses);
    }

    [Fact]
    public async Task Geocode_SpecialCharacters_ReturnsResults()
    {
        var addresses = await _asyncGeocoder.GeocodeAsync("Fried St & 2nd St, Gretna, LA 70053", TestContext.Current.CancellationToken);
        Assert.NotEmpty(addresses);
    }

    [Fact]
    public async Task Geocode_UnicodeCharacters_ReturnsResults()
    {
        var addresses = await _asyncGeocoder.GeocodeAsync("Étretat, France", TestContext.Current.CancellationToken);
        Assert.NotEmpty(addresses);
    }

    [Fact]
    public async Task ReverseGeocode_WhiteHouseCoordinates_ReturnsExpectedResult()
    {
        var addresses = await _asyncGeocoder.ReverseGeocodeAsync(38.8976777, -77.036517, TestContext.Current.CancellationToken);
        addresses.First().AssertWhiteHouse();
    }
}
