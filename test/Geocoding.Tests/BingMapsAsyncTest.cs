using Geocoding.Microsoft;
using Xunit;

namespace Geocoding.Tests;

[Collection("Settings")]
public class BingMapsAsyncTest : AsyncGeocoderTest
{
    private BingMapsGeocoder _bingMapsGeocoder;

    public BingMapsAsyncTest(SettingsFixture settings)
        : base(settings) { }

    protected override IGeocoder CreateAsyncGeocoder()
    {
        SettingsFixture.SkipIfMissing(_settings.BingMapsKey, nameof(SettingsFixture.BingMapsKey));
        _bingMapsGeocoder = new BingMapsGeocoder(_settings.BingMapsKey);
        return _bingMapsGeocoder;
    }

    [Theory]
    [InlineData("United States", EntityType.CountryRegion)]
    [InlineData("Illinois, US", EntityType.AdminDivision1)]
    [InlineData("New York, New York", EntityType.PopulatedPlace)]
    [InlineData("90210, US", EntityType.Postcode1)]
    [InlineData("1600 pennsylvania ave washington dc", EntityType.Address)]
    public async Task Geocode_AddressInput_ReturnsCorrectEntityType(string address, EntityType type)
    {
        var result = await _bingMapsGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken);
        var addresses = result.ToArray();
        Assert.Equal(type, addresses[0].Type);
    }
}
