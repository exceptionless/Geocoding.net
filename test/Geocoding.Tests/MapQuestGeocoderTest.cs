using Geocoding.MapQuest;
using Xunit;

namespace Geocoding.Tests;

[Collection("Settings")]
public class MapQuestGeocoderTest : GeocoderTest
{
    private MapQuestGeocoder _mapQuestGeocoder;

    public MapQuestGeocoderTest(SettingsFixture settings)
        : base(settings) { }

    protected override IGeocoder CreateGeocoder()
    {
        SettingsFixture.SkipIfMissing(_settings.MapQuestKey, nameof(SettingsFixture.MapQuestKey));
        _mapQuestGeocoder = new MapQuestGeocoder(_settings.MapQuestKey)
        {
            UseOSM = false
        };
        return _mapQuestGeocoder;
    }

    [Fact]
    public virtual async Task CanGeocodeNeighborhood()
    {
        // Regression test: Addresses with Quality=NEIGHBORHOOD are not returned
        var addresses = (await _mapQuestGeocoder.GeocodeAsync("North Sydney, New South Wales, Australia", TestContext.Current.CancellationToken)).ToArray();
        Assert.NotEmpty(addresses);
    }

}
