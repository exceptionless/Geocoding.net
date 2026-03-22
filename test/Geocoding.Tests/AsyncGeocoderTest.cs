using System.Globalization;
using Xunit;

namespace Geocoding.Tests;

public abstract class AsyncGeocoderTest
{
    private IGeocoder? _asyncGeocoder;
    protected readonly SettingsFixture _settings;

    protected AsyncGeocoderTest(SettingsFixture settings)
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-us");

        _settings = settings;
    }

    protected abstract IGeocoder CreateAsyncGeocoder();

    private IGeocoder GetGeocoder()
    {
        return _asyncGeocoder ??= CreateAsyncGeocoder();
    }

    protected TGeocoder GetGeocoder<TGeocoder>() where TGeocoder : class, IGeocoder
    {
        return GetGeocoder() as TGeocoder ?? throw new InvalidOperationException($"Expected geocoder of type {typeof(TGeocoder).Name}.");
    }

    [Fact]
    public async Task Geocode_ValidAddress_ReturnsExpectedResult()
    {
        // Act
        var addresses = await GetGeocoder().GeocodeAsync("1600 pennsylvania ave nw, washington dc", TestContext.Current.CancellationToken);

        // Assert
        addresses.First().AssertWhiteHouse();
    }

    [Fact]
    public async Task Geocode_NormalizedAddress_ReturnsExpectedResult()
    {
        // Act
        var addresses = await GetGeocoder().GeocodeAsync("1600 pennsylvania ave nw", "washington", "dc", null!, null!, TestContext.Current.CancellationToken);

        // Assert
        addresses.First().AssertWhiteHouse();
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("cs-CZ")]
    public async Task Geocode_DifferentCulture_ReturnsExpectedResult(string cultureName)
    {
        // Arrange
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

        // Act
        var addresses = await GetGeocoder().GeocodeAsync("24 sussex drive ottawa, ontario", TestContext.Current.CancellationToken);

        // Assert
        addresses.First().AssertCanadianPrimeMinister();
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("cs-CZ")]
    public async Task ReverseGeocode_DifferentCulture_ReturnsExpectedResult(string cultureName)
    {
        // Arrange
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);

        // Act
        var addresses = await GetGeocoder().ReverseGeocodeAsync(38.8976777, -77.036517, TestContext.Current.CancellationToken);

        // Assert
        addresses.First().AssertWhiteHouseArea();
    }

    [Fact]
    public async Task Geocode_InvalidAddress_ReturnsEmpty()
    {
        // Act
        var addresses = await GetGeocoder().GeocodeAsync("sdlkf;jasl;kjfldksjfasldf", TestContext.Current.CancellationToken);

        // Assert
        Assert.Empty(addresses);
    }

    [Fact]
    public async Task Geocode_SpecialCharacters_ReturnsResults()
    {
        // Act
        var addresses = await GetGeocoder().GeocodeAsync("Fried St & 2nd St, Gretna, LA 70053", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotEmpty(addresses);
    }

    [Fact]
    public async Task Geocode_UnicodeCharacters_ReturnsResults()
    {
        // Act
        var addresses = await GetGeocoder().GeocodeAsync("Étretat, France", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotEmpty(addresses);
    }

    [Fact]
    public async Task ReverseGeocode_WhiteHouseCoordinates_ReturnsExpectedResult()
    {
        // Act
        var addresses = await GetGeocoder().ReverseGeocodeAsync(38.8976777, -77.036517, TestContext.Current.CancellationToken);

        // Assert
        addresses.First().AssertWhiteHouse();
    }
}
