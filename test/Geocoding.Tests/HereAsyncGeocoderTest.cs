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
        SettingsFixture.SkipIfMissing(_settings.HereApiKey, nameof(SettingsFixture.HereApiKey));
        return new HereGeocoder(_settings.HereApiKey);
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
    public void Constructor_LegacyAppIdAppCode_ThrowsNotSupportedException()
    {
        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(() => new HereGeocoder("legacy-app-id", "legacy-app-code"));
        Assert.Contains("API key", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
