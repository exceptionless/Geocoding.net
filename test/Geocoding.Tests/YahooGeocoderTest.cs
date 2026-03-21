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
        Assert.Skip("Yahoo PlaceFinder/BOSS remains deprecated and unverified in this branch; see docs/plan.md and upstream issue #27.");
        return default!;
    }
}
