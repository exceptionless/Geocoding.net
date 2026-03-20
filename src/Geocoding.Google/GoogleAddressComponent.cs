﻿using System;

namespace Geocoding.Google
{
	/// <summary>
	/// Represents a component within a Google geocoding result.
	/// </summary>
	public class GoogleAddressComponent
	{
		/// <summary>
		/// Gets the Google address types associated with the component.
		/// </summary>
		public GoogleAddressType[] Types { get; private set; }
		/// <summary>
		/// Gets the long name for the component.
		/// </summary>
		public string LongName { get; private set; }
		/// <summary>
		/// Gets the short name for the component.
		/// </summary>
		public string ShortName { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GoogleAddressComponent"/> class.
		/// </summary>
		/// <param name="types">The Google address types for the component.</param>
		/// <param name="longName">The long component name.</param>
		/// <param name="shortName">The short component name.</param>
		public GoogleAddressComponent(GoogleAddressType[] types, string longName, string shortName)
		{
			if (types == null)
				throw new ArgumentNullException("types");

			if (types.Length < 1)
				throw new ArgumentException("Value cannot be empty.", "types");

			this.Types = types;
			this.LongName = longName;
			this.ShortName = shortName;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return string.Format("{0}: {1}", Types[0], LongName);
		}
	}
}