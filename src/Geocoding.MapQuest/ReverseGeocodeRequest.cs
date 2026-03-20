﻿using System;
using Newtonsoft.Json;

namespace Geocoding.MapQuest
{
	/// <summary>
	/// Represents a reverse geocoding request for MapQuest.
	/// </summary>
	public class ReverseGeocodeRequest : BaseRequest
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReverseGeocodeRequest"/> class.
		/// </summary>
		/// <param name="key">The MapQuest application key.</param>
		/// <param name="latitude">The latitude to reverse geocode.</param>
		/// <param name="longitude">The longitude to reverse geocode.</param>
		public ReverseGeocodeRequest(string key, double latitude, double longitude)
			: this(key, new Location(latitude, longitude)) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ReverseGeocodeRequest"/> class.
		/// </summary>
		/// <param name="key">The MapQuest application key.</param>
		/// <param name="loc">The coordinates to reverse geocode.</param>
		public ReverseGeocodeRequest(string key, Location loc)
			: this(key, new LocationRequest(loc)) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ReverseGeocodeRequest"/> class.
		/// </summary>
		/// <param name="key">The MapQuest application key.</param>
		/// <param name="loc">The request payload.</param>
		public ReverseGeocodeRequest(string key, LocationRequest loc)
			: base(key)
		{
			Location = loc;
		}

		[JsonIgnore]
		LocationRequest loc;
		/// <summary>
		/// Latitude and longitude for the request
		/// </summary>
		[JsonProperty("location")]
		public virtual LocationRequest Location
		{
			get { return loc; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("Location");

				loc = value;
			}
		}

		/// <inheritdoc />
		[JsonIgnore]
		public override string RequestAction
		{
			get { return "reverse"; }
		}
	}
}
