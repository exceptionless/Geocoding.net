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

}
