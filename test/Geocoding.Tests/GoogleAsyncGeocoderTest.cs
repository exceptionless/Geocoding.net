using Geocoding.Google;
using Xunit;

namespace Geocoding.Tests;

[Collection("Settings")]
public class GoogleAsyncGeocoderTest : AsyncGeocoderTest
{
    private GoogleGeocoder _googleGeocoder;

    public GoogleAsyncGeocoderTest(SettingsFixture settings)
        : base(settings) { }

    protected override IGeocoder CreateAsyncGeocoder()
    {
        String apiKey = _settings.GoogleApiKey;
        SettingsFixture.SkipIfMissing(apiKey, nameof(SettingsFixture.GoogleApiKey));
        _googleGeocoder = new GoogleGeocoder(apiKey);

        return _googleGeocoder;
    }

    [Theory]
    [InlineData("United States", GoogleAddressType.Country)]
    [InlineData("Illinois, US", GoogleAddressType.AdministrativeAreaLevel1)]
    [InlineData("New York, New York", GoogleAddressType.Locality)]
    [InlineData("90210, US", GoogleAddressType.PostalCode)]
    [InlineData("1600 pennsylvania ave washington dc", GoogleAddressType.Establishment)]
    public async Task Geocode_AddressInput_ReturnsCorrectAddressType(string address, GoogleAddressType type)
    {
        var result = await _googleGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken);
        var addresses = result.ToArray();
        Assert.Equal(type, addresses[0].Type);
    }
}
