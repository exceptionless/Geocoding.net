﻿namespace Geocoding.MapQuest;

 /// <summary>
 /// Represents a single-address geocoding request for MapQuest.
 /// </summary>
 public class GeocodeRequest : ReverseGeocodeRequest
 {
	 /// <summary>
	 /// Initializes a new instance of the <see cref="GeocodeRequest"/> class.
	 /// </summary>
	 /// <param name="key">The MapQuest application key.</param>
	 /// <param name="address">The address to geocode.</param>
	 public GeocodeRequest(string key, string address)
		 : this(key, new LocationRequest(address))
	 {
	 }

	 /// <summary>
	 /// Initializes a new instance of the <see cref="GeocodeRequest"/> class.
	 /// </summary>
	 /// <param name="key">The MapQuest application key.</param>
	 /// <param name="loc">The location request payload.</param>
	 public GeocodeRequest(string key, LocationRequest loc)
		 : base(key, loc)
	 {
	 }

	 /// <inheritdoc />
	 public override string RequestAction
	 {
		 get { return "address"; }
	 }
 }