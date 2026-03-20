﻿namespace Geocoding.Here;

 /// <summary>
 /// Represents the match type returned by the HERE geocoding service.
 /// </summary>
 /// <remarks>
 /// https://developer.here.com/documentation/geocoder/topics/resource-type-response-geocode.html
 /// </remarks>
 public enum HereMatchType
 {
	 /// <summary>The Unknown value.</summary>
	 Unknown,
	 /// <summary>The PointAddress value.</summary>
	 PointAddress,
	 /// <summary>The Interpolated value.</summary>
	 Interpolated
 }