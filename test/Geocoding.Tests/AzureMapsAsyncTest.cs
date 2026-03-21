using Geocoding.Microsoft;
using Xunit;

namespace Geocoding.Tests;

[Collection("Settings")]
public class AzureMapsAsyncTest : AsyncGeocoderTest
{
    public AzureMapsAsyncTest(SettingsFixture settings)
        : base(settings)
    {
    }

    protected override IGeocoder CreateAsyncGeocoder()
    {
        SettingsFixture.SkipIfMissing(_settings.AzureMapsKey, nameof(SettingsFixture.AzureMapsKey));
        return new AzureMapsGeocoder(_settings.AzureMapsKey);
    }

    [Theory]
    [InlineData("1600 pennsylvania ave washington dc", EntityType.Address)]
    [InlineData("United States", EntityType.CountryRegion)]
    public async Task Geocode_AddressInput_ReturnsCorrectEntityType(string address, EntityType type)
    {
        // Arrange
        var geocoder = (AzureMapsGeocoder)CreateAsyncGeocoder();

        // Act
        var results = (await geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.Equal(type, results[0].Type);
    }
}