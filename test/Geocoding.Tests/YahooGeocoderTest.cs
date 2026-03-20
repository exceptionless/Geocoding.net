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
        SettingsFixture.SkipIfMissing(_settings.YahooConsumerKey, nameof(SettingsFixture.YahooConsumerKey));
        SettingsFixture.SkipIfMissing(_settings.YahooConsumerSecret, nameof(SettingsFixture.YahooConsumerSecret));
        return new YahooGeocoder(_settings.YahooConsumerKey, _settings.YahooConsumerSecret);
    }
}
