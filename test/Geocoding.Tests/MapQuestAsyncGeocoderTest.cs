using Geocoding.MapQuest;
using Xunit;

namespace Geocoding.Tests;

[Collection("Settings")]
public class MapQuestAsyncGeocoderTest : AsyncGeocoderTest
{
    public MapQuestAsyncGeocoderTest(SettingsFixture settings)
        : base(settings) { }

    protected override IGeocoder CreateAsyncGeocoder()
    {
        SettingsFixture.SkipIfMissing(_settings.MapQuestKey, nameof(SettingsFixture.MapQuestKey));
        return new MapQuestGeocoder(_settings.MapQuestKey)
        {
            UseOSM = false
        };
    }
}
