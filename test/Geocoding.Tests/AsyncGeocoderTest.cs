using System.Globalization;
using Xunit;

namespace Geocoding.Tests;

public abstract class AsyncGeocoderTest
{
	private readonly IGeocoder _asyncGeocoder;
	protected readonly SettingsFixture _settings;

	protected AsyncGeocoderTest(SettingsFixture settings)
	{
		CultureInfo.CurrentCulture = new CultureInfo("en-us");

		_settings = settings;
		_asyncGeocoder = CreateAsyncGeocoder();
	}

	protected abstract IGeocoder CreateAsyncGeocoder();

	[Fact]
	public async Task CanGeocodeAddress()
	{
		var addresses = await _asyncGeocoder.GeocodeAsync("1600 pennsylvania ave washington dc", TestContext.Current.CancellationToken);
		addresses.First().AssertWhiteHouse();
	}

	[Fact]
	public async Task CanGeocodeNormalizedAddress()
	{
		var addresses = await _asyncGeocoder.GeocodeAsync("1600 pennsylvania ave", "washington", "dc", null, null, TestContext.Current.CancellationToken);
		addresses.First().AssertWhiteHouse();
	}

	[Theory]
	[InlineData("en-US")]
	[InlineData("cs-CZ")]
	public async Task CanGeocodeAddressUnderDifferentCultures(string cultureName)
	{
		CultureInfo.CurrentCulture = new CultureInfo(cultureName);

		var addresses = await _asyncGeocoder.GeocodeAsync("24 sussex drive ottawa, ontario", TestContext.Current.CancellationToken);
		addresses.First().AssertCanadianPrimeMinister();
	}

	[Theory]
	[InlineData("en-US")]
	[InlineData("cs-CZ")]
	public async Task CanReverseGeocodeAddressUnderDifferentCultures(string cultureName)
	{
		CultureInfo.CurrentCulture = new CultureInfo(cultureName);

		var addresses = await _asyncGeocoder.ReverseGeocodeAsync(38.8976777, -77.036517, TestContext.Current.CancellationToken);
		addresses.First().AssertWhiteHouseArea();
	}

	[Fact]
	public async Task ShouldNotBlowUpOnBadAddress()
	{
		var addresses = await _asyncGeocoder.GeocodeAsync("sdlkf;jasl;kjfldksjfasldf", TestContext.Current.CancellationToken);
		Assert.Empty(addresses);
	}

	[Fact]
	public async Task CanGeocodeWithSpecialCharacters()
	{
		var addresses = await _asyncGeocoder.GeocodeAsync("Fried St & 2nd St, Gretna, LA 70053", TestContext.Current.CancellationToken);
		Assert.NotEmpty(addresses);
	}

	[Fact]
	public async Task CanGeocodeWithUnicodeCharacters()
	{
		var addresses = await _asyncGeocoder.GeocodeAsync("Étretat, France", TestContext.Current.CancellationToken);
		Assert.NotEmpty(addresses);
	}

	[Fact]
	public async Task CanReverseGeocodeAsync()
	{
		var addresses = await _asyncGeocoder.ReverseGeocodeAsync(38.8976777, -77.036517, TestContext.Current.CancellationToken);
		addresses.First().AssertWhiteHouse();
	}
}
