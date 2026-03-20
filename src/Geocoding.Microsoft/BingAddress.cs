﻿namespace Geocoding.Microsoft
{
	/// <summary>
	/// Represents an address returned by the Bing Maps geocoding service.
	/// </summary>
	public class BingAddress : Address
	{
		readonly string addressLine, adminDistrict, adminDistrict2, countryRegion, locality, neighborhood, postalCode;
		readonly EntityType type;
		readonly ConfidenceLevel confidence;

		/// <summary>
		/// Gets the street address line.
		/// </summary>
		public string AddressLine
		{
			get { return addressLine ?? ""; }
		}

		/// <summary>
		/// Gets the primary administrative district.
		/// </summary>
		public string AdminDistrict
		{
			get { return adminDistrict ?? ""; }
		}

		/// <summary>
		/// Gets the secondary administrative district.
		/// </summary>
		public string AdminDistrict2
		{
			get { return adminDistrict2 ?? ""; }
		}

		/// <summary>
		/// Gets the country or region.
		/// </summary>
		public string CountryRegion
		{
			get { return countryRegion ?? ""; }
		}

		/// <summary>
		/// Gets the locality.
		/// </summary>
		public string Locality
		{
			get { return locality ?? ""; }
		}

		/// <summary>
		/// Gets the neighborhood.
		/// </summary>
		public string Neighborhood
		{
			get { return neighborhood ?? ""; }
		}

		/// <summary>
		/// Gets the postal code.
		/// </summary>
		public string PostalCode
		{
			get { return postalCode ?? ""; }
		}

		/// <summary>
		/// Gets the Bing Maps entity type.
		/// </summary>
		public EntityType Type
		{
			get { return type; }
		}

		/// <summary>
		/// Gets the Bing Maps confidence level.
		/// </summary>
		public ConfidenceLevel Confidence
		{
			get { return confidence; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BingAddress"/> class.
		/// </summary>
		/// <param name="formattedAddress">The formatted address returned by Bing Maps.</param>
		/// <param name="coordinates">The coordinates returned by Bing Maps.</param>
		/// <param name="addressLine">The street address line.</param>
		/// <param name="adminDistrict">The primary administrative district.</param>
		/// <param name="adminDistrict2">The secondary administrative district.</param>
		/// <param name="countryRegion">The country or region.</param>
		/// <param name="locality">The locality.</param>
		/// <param name="neighborhood">The neighborhood.</param>
		/// <param name="postalCode">The postal code.</param>
		/// <param name="type">The entity type returned by Bing Maps.</param>
		/// <param name="confidence">The confidence level returned by Bing Maps.</param>
		public BingAddress(string formattedAddress, Location coordinates, string addressLine, string adminDistrict, string adminDistrict2,
			string countryRegion, string locality, string neighborhood, string postalCode, EntityType type, ConfidenceLevel confidence)
			: base(formattedAddress, coordinates, "Bing")
		{
			this.addressLine = addressLine;
			this.adminDistrict = adminDistrict;
			this.adminDistrict2 = adminDistrict2;
			this.countryRegion = countryRegion;
			this.locality = locality;
			this.neighborhood = neighborhood;
			this.postalCode = postalCode;
			this.type = type;
			this.confidence = confidence;
		}
	}
}