﻿using Geocoding.Yahoo;
 using Xunit;

 namespace Geocoding.Tests;

 [Collection("Settings")]
 public class YahooGeocoderTest : GeocoderTest
 {
	 public YahooGeocoderTest(SettingsFixture settings)
		 : base(settings) { }

	 protected override IGeocoder CreateGeocoder()
	 {
		 SettingsFixture.SkipIfMissing(_settings.YahooConsumerKey, nameof(SettingsFixture.YahooConsumerKey));
		 SettingsFixture.SkipIfMissing(_settings.YahooConsumerSecret, nameof(SettingsFixture.YahooConsumerSecret));

		 return new YahooGeocoder(
			 _settings.YahooConsumerKey,
			 _settings.YahooConsumerSecret
		 );
	 }

	 //TODO: delete these when tests are ready to be unskipped
	 //see issue #27

	 [Theory(Skip = "oauth not working for yahoo - see issue #27")]
	 [MemberData(nameof(AddressData), MemberType = typeof(GeocoderTest))]
	 public override Task CanGeocodeAddress(string address)
	 {
		 return base.CanGeocodeAddress(address);
	 }

	 [Fact(Skip = "oauth not working for yahoo - see issue #27")]
	 public override Task CanGeocodeNormalizedAddress()
	 {
		 return base.CanGeocodeNormalizedAddress();
	 }

	 [Theory(Skip = "oauth not working for yahoo - see issue #27")]
	 [MemberData(nameof(CultureData), MemberType = typeof(GeocoderTest))]
	 public override Task CanGeocodeAddressUnderDifferentCultures(string cultureName)
	 {
		 return base.CanGeocodeAddressUnderDifferentCultures(cultureName);
	 }

	 [Theory(Skip = "oauth not working for yahoo - see issue #27")]
	 [MemberData(nameof(CultureData), MemberType = typeof(GeocoderTest))]
	 public override Task CanReverseGeocodeAddressUnderDifferentCultures(string cultureName)
	 {
		 return base.CanReverseGeocodeAddressUnderDifferentCultures(cultureName);
	 }

	 [Fact(Skip = "oauth not working for yahoo - see issue #27")]
	 public override Task ShouldNotBlowUpOnBadAddress()
	 {
		 return base.ShouldNotBlowUpOnBadAddress();
	 }

	 [Theory(Skip = "oauth not working for yahoo - see issue #27")]
	 [MemberData(nameof(SpecialCharacterAddressData), MemberType = typeof(GeocoderTest))]
	 public override Task CanGeocodeWithSpecialCharacters(string address)
	 {
		 return base.CanGeocodeWithSpecialCharacters(address);
	 }

	 [Fact(Skip = "oauth not working for yahoo - see issue #27")]
	 public override Task CanReverseGeocodeAsync()
	 {
		 return base.CanReverseGeocodeAsync();
	 }

	 [Theory(Skip = "oauth not working for yahoo - see issue #27")]
	 [MemberData(nameof(InvalidZipCodeAddressData), MemberType = typeof(GeocoderTest))]
	 public override Task CanGeocodeInvalidZipCodes(string address)
	 {
		 return base.CanGeocodeInvalidZipCodes(address);
	 }

	 [Theory(Skip = "oauth not working for yahoo - see issue #27")]
	 [MemberData(nameof(StreetIntersectionAddressData), MemberType = typeof(GeocoderTest))]
	 public override Task CanHandleStreetIntersectionsByAmpersand(string address)
	 {
		 return base.CanHandleStreetIntersectionsByAmpersand(address);
	 }
 }