﻿namespace Geocoding.Here;

 /// <summary>
 /// Represents an address returned by the HERE geocoding service.
 /// </summary>
 public class HereAddress : Address
 {
	 readonly string street, houseNumber, city, state, country, postalCode;
	 readonly HereLocationType type;

	 /// <summary>
	 /// Gets the street portion of the HERE result.
	 /// </summary>
	 public string AddressLine
	 {
		 get { return street ?? ""; }
	 }

	 /// <summary>
	 /// Gets the provider-specific secondary street detail.
	 /// </summary>
	 public string AdminDistrict
	 {
		 get { return houseNumber ?? ""; }
	 }

	 /// <summary>
	 /// Gets the city portion of the HERE result.
	 /// </summary>
	 public string AdminDistrict2
	 {
		 get { return city ?? ""; }
	 }

	 /// <summary>
	 /// Gets the state or region portion of the HERE result.
	 /// </summary>
	 public string CountryRegion
	 {
		 get { return state ?? ""; }
	 }

	 /// <summary>
	 /// Gets the country portion of the HERE result.
	 /// </summary>
	 public string Neighborhood
	 {
		 get { return country ?? ""; }
	 }

	 /// <summary>
	 /// Gets the postal code reported by HERE.
	 /// </summary>
	 public string PostalCode
	 {
		 get { return postalCode ?? ""; }
	 }

	 /// <summary>
	 /// Gets the HERE location classification for the result.
	 /// </summary>
	 public HereLocationType Type
	 {
		 get { return type; }
	 }

	 /// <summary>
	 /// Initializes a new instance of the <see cref="HereAddress"/> class.
	 /// </summary>
	 /// <param name="formattedAddress">The formatted address returned by HERE.</param>
	 /// <param name="coordinates">The coordinates returned by HERE.</param>
	 /// <param name="street">The street name.</param>
	 /// <param name="houseNumber">The house number.</param>
	 /// <param name="city">The city name.</param>
	 /// <param name="state">The state or region.</param>
	 /// <param name="postalCode">The postal code.</param>
	 /// <param name="country">The country name.</param>
	 /// <param name="type">The HERE location type.</param>
	 public HereAddress(string formattedAddress, Location coordinates, string street, string houseNumber, string city,
		 string state, string postalCode, string country, HereLocationType type)
		 : base(formattedAddress, coordinates, "HERE")
	 {
		 this.street = street;
		 this.houseNumber = houseNumber;
		 this.city = city;
		 this.state = state;
		 this.postalCode = postalCode;
		 this.country = country;
		 this.type = type;
	 }
 }