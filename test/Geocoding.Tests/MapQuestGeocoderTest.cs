using Geocoding.MapQuest;
using Xunit;

namespace Geocoding.Tests;

[Collection("Settings")]
public class MapQuestGeocoderTest : GeocoderTest
{
    public MapQuestGeocoderTest(SettingsFixture settings)
        : base(settings) { }

    protected override IGeocoder CreateGeocoder()
    {
        SettingsFixture.SkipIfMissing(_settings.MapQuestKey, nameof(SettingsFixture.MapQuestKey));
        return new MapQuestGeocoder(_settings.MapQuestKey)
        {
            UseOSM = false
        };
    }

    // Regression test: Addresses with Quality=NEIGHBORHOOD are not returned
    [Fact]
    public virtual async Task Geocode_NeighborhoodAddress_ReturnsResults()
    {
        var geocoder = GetGeocoder<MapQuestGeocoder>();

        // Act
        var addresses = (await geocoder.GeocodeAsync("North Sydney, New South Wales, Australia", TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.NotEmpty(addresses);
    }

    [Fact]
    public void UseOSM_SetTrue_ThrowsNotSupportedException()
    {
        // Arrange
        var geocoder = new MapQuestGeocoder("mapquest-key");

        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(() => geocoder.UseOSM = true);
        Assert.Contains("no longer supported", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
