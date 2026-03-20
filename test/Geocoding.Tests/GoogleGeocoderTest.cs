using Geocoding.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class GoogleGeocoderTest : GeocoderTest
	{
		private GoogleGeocoder _googleGeocoder;

		public GoogleGeocoderTest(SettingsFixture settings)
			: base(settings) { }

		protected override IGeocoder CreateGeocoder()
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
		[InlineData("muswellbrook 2 New South Wales Australia", GoogleAddressType.Unknown)]
		public async Task CanParseAddressTypes(string address, GoogleAddressType type)
		{
			var addresses = (await _googleGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();
			Assert.Equal(type, addresses[0].Type);
		}

		[Theory]
		[InlineData("United States", GoogleLocationType.Approximate)]
		[InlineData("Illinois, US", GoogleLocationType.Approximate)]
		[InlineData("Ingalls Corners Road, Canastota, NY 13032, USA", GoogleLocationType.GeometricCenter)]
		[InlineData("51 Harry S. Truman Parkway, Annapolis, MD 21401, USA", GoogleLocationType.RangeInterpolated)]
		[InlineData("1600 pennsylvania ave washington dc", GoogleLocationType.Rooftop)]
		[InlineData("muswellbrook 2 New South Wales Australia", GoogleLocationType.Approximate)]
		public async Task CanParseLocationTypes(string address, GoogleLocationType type)
		{
			var addresses = (await _googleGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();
			Assert.Equal(type, addresses[0].LocationType);
		}

		[Theory]
		[InlineData("United States", "fr", "États-Unis")]
		[InlineData("Montreal", "en", "Montreal, QC, Canada")]
		[InlineData("Montreal", "fr", "Montréal, QC, Canada")]
		[InlineData("Montreal", "de", "Montreal, Québec, Kanada")]
		public async Task ApplyLanguage(string address, string language, string result)
		{
			_googleGeocoder.Language = language;
			var addresses = (await _googleGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();
			Assert.Equal(result, addresses[0].FormattedAddress);
		}

		[Theory]
		[InlineData("Toledo", "us", "Toledo, OH, USA", null)]
		[InlineData("Toledo", "es", "Toledo, Spain", "Toledo, Toledo, Spain")]
		public async Task ApplyRegionBias(string address, string regionBias, string result1, string result2)
		{
			_googleGeocoder.RegionBias = regionBias;
			var addresses = (await _googleGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();
			String[] expectedAddresses = String.IsNullOrEmpty(result2) ? new[] { result1 } : new[] { result1, result2 };
			Assert.Contains(addresses[0].FormattedAddress, expectedAddresses);
		}

		[Theory]
		[InlineData("Winnetka", 46, -90, 47, -91, "Winnetka, IL, USA")]
		[InlineData("Winnetka", 34.172684, -118.604794, 34.236144, -118.500938, "Winnetka, Los Angeles, CA, USA")]
		public async Task ApplyBoundsBias(string address, double biasLatitude1, double biasLongitude1, double biasLatitude2, double biasLongitude2, string result)
		{
			_googleGeocoder.BoundsBias = new Bounds(biasLatitude1, biasLongitude1, biasLatitude2, biasLongitude2);
			var addresses = (await _googleGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();
			Assert.Equal(result, addresses[0].FormattedAddress);
		}

		[Theory]
		[InlineData("Wimbledon")]
		[InlineData("Birmingham")]
		[InlineData("Manchester")]
		[InlineData("York")]
		public async Task CanApplyGBCountryComponentFilters(string address)
		{
			_googleGeocoder.ComponentFilters = new List<GoogleComponentFilter>();

			_googleGeocoder.ComponentFilters.Add(new GoogleComponentFilter(GoogleComponentFilterType.Country, "GB"));

			var addresses = (await _googleGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

			Assert.DoesNotContain(addresses, x => HasShortName(x, "US"));
			Assert.Contains(addresses, x => HasShortName(x, "GB"));
		}

		[Theory]
		[InlineData("Wimbledon")]
		[InlineData("Birmingham")]
		[InlineData("Manchester")]
		[InlineData("York")]
		public async Task CanApplyUSCountryComponentFilters(string address)
		{
			_googleGeocoder.ComponentFilters = new List<GoogleComponentFilter>();

			_googleGeocoder.ComponentFilters.Add(new GoogleComponentFilter(GoogleComponentFilterType.Country, "US"));

			var addresses = (await _googleGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

			Assert.Contains(addresses, x => HasShortName(x, "US"));
			Assert.DoesNotContain(addresses, x => HasShortName(x, "GB"));
		}

		[Theory]
		[InlineData("Washington")]
		[InlineData("Franklin")]
		public async Task CanApplyAdministrativeAreaComponentFilters(string address)
		{
			_googleGeocoder.ComponentFilters = new List<GoogleComponentFilter>();

			_googleGeocoder.ComponentFilters.Add(new GoogleComponentFilter(GoogleComponentFilterType.AdministrativeArea, "KS"));

			var addresses = (await _googleGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

			// Assert we only got addresses in Kansas
			Assert.Contains(addresses, x => HasShortName(x, "KS"));
			Assert.DoesNotContain(addresses, x => HasShortName(x, "MA"));
			Assert.DoesNotContain(addresses, x => HasShortName(x, "LA"));
			Assert.DoesNotContain(addresses, x => HasShortName(x, "NJ"));
		}

		[Theory]
		[InlineData("Rothwell")]
		public async Task CanApplyPostalCodeComponentFilters(string address)
		{
			_googleGeocoder.ComponentFilters = new List<GoogleComponentFilter>();

			_googleGeocoder.ComponentFilters.Add(new GoogleComponentFilter(GoogleComponentFilterType.PostalCode, "NN14"));

			var addresses = (await _googleGeocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

			// Assert we only got Rothwell, Northamptonshire
			Assert.Contains(addresses, x => HasShortName(x, "Northamptonshire"));
			Assert.DoesNotContain(addresses, x => HasShortName(x, "West Yorkshire"));
			Assert.DoesNotContain(addresses, x => HasShortName(x, "Moreton Bay"));
		}

		private static bool HasShortName(GoogleAddress address, string shortName)
		{
			return address.Components.Any(component => String.Equals(component.ShortName, shortName, StringComparison.Ordinal));
		}
	}
}