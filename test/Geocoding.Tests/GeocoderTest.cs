using System.Globalization;
using Xunit;

namespace Geocoding.Tests;

public abstract class GeocoderTest
{
    public static IEnumerable<object[]> AddressData => new[] {
        new object[] { "1600 pennsylvania ave nw, washington dc" }
    };

    public static IEnumerable<object[]> CultureData => new[] {
        new object[] { "en-US" },
        new object[] { "cs-CZ" }
    };

    public static IEnumerable<object[]> SpecialCharacterAddressData => new[] {
        new object[] { "40 1/2 Road" },
        new object[] { "B's Farm RD" },
        new object[] { "Wilshire & Bundy Plaza, Los Angeles" },
        new object[] { "Étretat, France" }
    };

    public static IEnumerable<object[]> StreetIntersectionAddressData => new[] {
        new object[] { "Wilshire & Centinela, Los Angeles" },
        new object[] { "Fried St & 2nd St, Gretna, LA 70053" }
    };

    public static IEnumerable<object[]> InvalidZipCodeAddressData => new[] {
        new object[] { "1 Robert Wood Johnson Hosp New Brunswick, NJ 08901 USA" },
        new object[] { "miss, MO" }
    };

    private IGeocoder? _geocoder;
    protected readonly SettingsFixture _settings;

    public GeocoderTest(SettingsFixture settings)
    {
        //Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-us");

        _settings = settings;
    }

    protected abstract IGeocoder CreateGeocoder();

    private IGeocoder GetGeocoder()
    {
        return _geocoder ??= CreateGeocoder();
    }

    protected TGeocoder GetGeocoder<TGeocoder>() where TGeocoder : class, IGeocoder
    {
        return GetGeocoder() as TGeocoder ?? throw new InvalidOperationException($"Expected geocoder of type {typeof(TGeocoder).Name}.");
    }

    protected static async Task RunInCultureAsync(string cultureName, Func<Task> action)
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        CultureInfo originalUICulture = CultureInfo.CurrentUICulture;
        CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);

        try
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            await action();
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUICulture;
        }
    }

    [Theory]
    [MemberData(nameof(AddressData))]
    public virtual async Task Geocode_ValidAddress_ReturnsExpectedResult(string address)
    {
        // Act
        var addresses = (await GetGeocoder().GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        addresses[0].AssertWhiteHouse();
    }

    [Fact]
    public virtual async Task Geocode_NormalizedAddress_ReturnsExpectedResult()
    {
        // Act
        var addresses = (await GetGeocoder().GeocodeAsync("1600 pennsylvania ave nw", "washington", "dc", null!, null!, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        addresses[0].AssertWhiteHouse();
    }

    [Theory]
    [MemberData(nameof(CultureData))]
    public virtual Task Geocode_DifferentCulture_ReturnsExpectedResult(string cultureName)
    {
        return RunInCultureAsync(cultureName, async () =>
        {
            // Arrange
            Assert.Equal(cultureName, CultureInfo.CurrentCulture.Name);

            // Act
            var addresses = (await GetGeocoder().GeocodeAsync("24 sussex drive ottawa, ontario", TestContext.Current.CancellationToken)).ToArray();

            // Assert
            addresses[0].AssertCanadianPrimeMinister();
        });
    }

    [Theory]
    [MemberData(nameof(CultureData))]
    public virtual Task ReverseGeocode_DifferentCulture_ReturnsExpectedResult(string cultureName)
    {
        return RunInCultureAsync(cultureName, async () =>
        {
            // Arrange
            Assert.Equal(cultureName, CultureInfo.CurrentCulture.Name);

            // Act
            var addresses = (await GetGeocoder().ReverseGeocodeAsync(38.8976777, -77.036517, TestContext.Current.CancellationToken)).ToArray();

            // Assert
            addresses[0].AssertWhiteHouseArea();
        });
    }

    [Fact]
    public virtual async Task Geocode_InvalidAddress_ReturnsEmpty()
    {
        // Act
        var addresses = (await GetGeocoder().GeocodeAsync("sdlkf;jasl;kjfldksj,fasldf", TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.Empty(addresses);
    }

    [Theory]
    [MemberData(nameof(SpecialCharacterAddressData))]
    public virtual async Task Geocode_SpecialCharacters_ReturnsResults(string address)
    {
        // Act
        var addresses = (await GetGeocoder().GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.NotEmpty(addresses);
    }

    [Theory]
    [MemberData(nameof(StreetIntersectionAddressData))]
    public virtual async Task Geocode_StreetIntersection_ReturnsResults(string address)
    {
        // Act
        var addresses = (await GetGeocoder().GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.NotEmpty(addresses);
    }

    [Fact]
    public virtual async Task ReverseGeocode_WhiteHouseCoordinates_ReturnsExpectedArea()
    {
        // Act
        var addresses = (await GetGeocoder().ReverseGeocodeAsync(38.8976777, -77.036517, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        addresses[0].AssertWhiteHouseArea();
    }

    [Theory]
    [MemberData(nameof(InvalidZipCodeAddressData))]
    //https://github.com/chadly/Geocoding.net/issues/6
    public virtual async Task Geocode_InvalidZipCode_ReturnsResults(string address)
    {
        // Act
        var addresses = (await GetGeocoder().GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.NotEmpty(addresses);
    }
}
