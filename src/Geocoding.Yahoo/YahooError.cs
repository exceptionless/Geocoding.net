﻿namespace Geocoding.Yahoo;

 /// <remarks>http://developer.yahoo.com/geo/placefinder/guide/responses.html#error-codes</remarks>
 public enum YahooError
 {
	 /// <summary>The NoError value.</summary>
	 NoError = 0,
	 /// <summary>The FeatureNotSupported value.</summary>
	 FeatureNotSupported = 1,
	 /// <summary>The NoInputParameters value.</summary>
	 NoInputParameters = 100,
	 /// <summary>The AddressNotUtf8 value.</summary>
	 AddressNotUtf8 = 102,
	 /// <summary>The InsufficientAddressData value.</summary>
	 InsufficientAddressData = 103,
	 /// <summary>The UnknownLanguage value.</summary>
	 UnknownLanguage = 104,
	 /// <summary>The NoCountryDetected value.</summary>
	 NoCountryDetected = 105,
	 /// <summary>The CountryNotSupported value.</summary>
	 CountryNotSupported = 106,
	 /// <summary>The UnknownError value.</summary>
	 UnknownError = 1000
 }