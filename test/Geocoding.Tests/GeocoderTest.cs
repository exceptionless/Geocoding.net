using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Geocoding.Tests
{
	public abstract class GeocoderTest
	{
		public static IEnumerable<object[]> AddressData => new[] {
			new object[] { "1600 pennsylvania ave nw, washington dc" }
		};

		public static IEnumerable<object[]> CultureData => new[] {
			new object[] { "en-US" },
			new object[] { "cs-CZ" }
		};

		public static IEnumerable<object[]> SpecialCharacterAddressData => new[] {
			new object[] { "40 1/2 Road" },
			new object[] { "B's Farm RD" },
			new object[] { "Wilshire & Bundy Plaza, Los Angeles" },
			new object[] { "Étretat, France" }
		};

		public static IEnumerable<object[]> StreetIntersectionAddressData => new[] {
			new object[] { "Wilshire & Centinela, Los Angeles" },
			new object[] { "Fried St & 2nd St, Gretna, LA 70053" }
		};

		public static IEnumerable<object[]> InvalidZipCodeAddressData => new[] {
			new object[] { "1 Robert Wood Johnson Hosp New Brunswick, NJ 08901 USA" },
			new object[] { "miss, MO" }
		};

		readonly IGeocoder geocoder;
		protected readonly SettingsFixture settings;

		public GeocoderTest(SettingsFixture settings)
		{
			//Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-us");

			this.settings = settings;
			geocoder = CreateGeocoder();
		}

		protected abstract IGeocoder CreateGeocoder();

		protected static async Task RunInCultureAsync(string cultureName, Func<Task> action)
		{
			CultureInfo originalCulture = CultureInfo.CurrentCulture;
			CultureInfo originalUICulture = CultureInfo.CurrentUICulture;
			CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);

			try {
				CultureInfo.CurrentCulture = culture;
				CultureInfo.CurrentUICulture = culture;
				await action();
			} finally {
				CultureInfo.CurrentCulture = originalCulture;
				CultureInfo.CurrentUICulture = originalUICulture;
			}
		}

		[Theory]
		[MemberData(nameof(AddressData))]
		public virtual async Task CanGeocodeAddress(string address)
		{
			Address[] addresses = (await geocoder.GeocodeAsync(address)).ToArray();
			addresses[0].AssertWhiteHouse();
		}

		[Fact]
		public virtual async Task CanGeocodeNormalizedAddress()
		{
			Address[] addresses = (await geocoder.GeocodeAsync("1600 pennsylvania ave nw", "washington", "dc", null, null)).ToArray();
			addresses[0].AssertWhiteHouse();
		}

		[Theory]
		[MemberData(nameof(CultureData))]
		public virtual async Task CanGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			await RunInCultureAsync(cultureName, async () => {
				Assert.Equal(cultureName, CultureInfo.CurrentCulture.Name);
				Address[] addresses = (await geocoder.GeocodeAsync("24 sussex drive ottawa, ontario")).ToArray();
				addresses[0].AssertCanadianPrimeMinister();
			});
		}

		[Theory]
		[MemberData(nameof(CultureData))]
		public virtual async Task CanReverseGeocodeAddressUnderDifferentCultures(string cultureName)
		{
			await RunInCultureAsync(cultureName, async () => {
				Assert.Equal(cultureName, CultureInfo.CurrentCulture.Name);
				Address[] addresses = (await geocoder.ReverseGeocodeAsync(38.8976777, -77.036517)).ToArray();
				addresses[0].AssertWhiteHouseArea();
			});
		}

		[Fact]
		public virtual async Task ShouldNotBlowUpOnBadAddress()
		{
			Address[] addresses = (await geocoder.GeocodeAsync("sdlkf;jasl;kjfldksj,fasldf")).ToArray();
			Assert.Empty(addresses);
		}

		[Theory]
		[MemberData(nameof(SpecialCharacterAddressData))]
		public virtual async Task CanGeocodeWithSpecialCharacters(string address)
		{
			Address[] addresses = (await geocoder.GeocodeAsync(address)).ToArray();

			//asserting no exceptions are thrown and that we get something
			Assert.NotEmpty(addresses);
		}

		[Theory]
		[MemberData(nameof(StreetIntersectionAddressData))]
		public virtual async Task CanHandleStreetIntersectionsByAmpersand(string address)
		{
			Address[] addresses = (await geocoder.GeocodeAsync(address)).ToArray();

			//asserting no exceptions are thrown and that we get something
			Assert.NotEmpty(addresses);
		}

		[Fact]
		public virtual async Task CanReverseGeocodeAsync()
		{
			Address[] addresses = (await geocoder.ReverseGeocodeAsync(38.8976777, -77.036517)).ToArray();
			addresses[0].AssertWhiteHouseArea();
		}

		[Theory]
		[MemberData(nameof(InvalidZipCodeAddressData))]
		//https://github.com/chadly/Geocoding.net/issues/6
		public virtual async Task CanGeocodeInvalidZipCodes(string address)
		{
			Address[] addresses = (await geocoder.GeocodeAsync(address)).ToArray();
			Assert.NotEmpty(addresses);
		}
	}
}
