using Geocoding.Microsoft;
using Xunit;

namespace Geocoding.Tests;

[Collection("Settings")]
public class BingMapsTest : GeocoderTest
{
	private BingMapsGeocoder _bingMapsGeocoder;

	public BingMapsTest(SettingsFixture settings)
		: base(settings) { }

	protected override IGeocoder CreateGeocoder()
	{
		SettingsFixture.SkipIfMissing(_settings.BingMapsKey, nameof(SettingsFixture.BingMapsKey));
		_bingMapsGeocoder = new BingMapsGeocoder(_settings.BingMapsKey);
		return _bingMapsGeocoder;
	}

	[Theory]
	[InlineData("United States", "fr", "États-Unis")]
	[InlineData("Montreal", "en", "Montreal, QC")]
	[InlineData("Montreal", "fr", "Montréal, QC")]
	public async Task ApplyCulture(string address, string culture, string result)
	{
		_bingMapsGeocoder.Culture = culture;
		var addresses = (await _bingMapsGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();
		Assert.Equal(result, addresses[0].FormattedAddress);
	}

	[Theory]
	[InlineData("Montreal", 45.512401580810547, -73.554679870605469, "Canada")]
	[InlineData("Montreal", 43.949058532714844, 0.20011000335216522, "France")]
	[InlineData("Montreal", 46.428329467773438, -90.241783142089844, "United States")]
	public async Task ApplyUserLocation(string address, double userLatitude, double userLongitude, string country)
	{
		_bingMapsGeocoder.UserLocation = new Location(userLatitude, userLongitude);
		var addresses = (await _bingMapsGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();
		Assert.Contains(addresses, x => String.Equals(x.CountryRegion, country, StringComparison.Ordinal));
	}

	[Theory]
	[InlineData("Montreal", 45, -73, 46, -74, "Canada")]
	[InlineData("Montreal", 43, 0, 44, 1, "France")]
	[InlineData("Montreal", 46, -90, 47, -91, "United States")]
	public async Task ApplyUserMapView(string address, double userLatitude1, double userLongitude1, double userLatitude2, double userLongitude2, string country)
	{
		_bingMapsGeocoder.UserMapView = new Bounds(userLatitude1, userLongitude1, userLatitude2, userLongitude2);
		_bingMapsGeocoder.MaxResults = 20;
		var addresses = (await _bingMapsGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();
		Assert.Contains(addresses, x => String.Equals(x.CountryRegion, country, StringComparison.Ordinal));
	}

	[Theory]
	[InlineData("24 sussex drive ottawa, ontario")]
	public async Task ApplyIncludeNeighborhood(string address)
	{
		_bingMapsGeocoder.IncludeNeighborhood = true;
		var addresses = (await _bingMapsGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();
		Assert.NotNull(addresses[0].Neighborhood);
	}

	[Fact]
	//https://github.com/chadly/Geocoding.net/issues/8
	public async Task CanReverseGeocodeIssue8()
	{
		var addresses = (await _bingMapsGeocoder.ReverseGeocodeAsync(38.8976777, -77.036517, TestContext.Current.CancellationToken)).ToArray();
		Assert.NotEmpty(addresses);
	}
}