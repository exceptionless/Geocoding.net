namespace Geocoding;

/// <summary>
/// Generic parsed address with each field separated out form the original FormattedAddress
/// </summary>
public class ParsedAddress : Address
{
	/// <summary>
	/// Gets or sets the street portion.
	/// </summary>
	public virtual string Street { get; set; }
	/// <summary>
	/// Gets or sets the city portion.
	/// </summary>
	public virtual string City { get; set; }
	/// <summary>
	/// Gets or sets the county portion.
	/// </summary>
	public virtual string County { get; set; }
	/// <summary>
	/// Gets or sets the state or region portion.
	/// </summary>
	public virtual string State { get; set; }
	/// <summary>
	/// Gets or sets the country portion.
	/// </summary>
	public virtual string Country { get; set; }
	/// <summary>
	/// Gets or sets the postal or zip code portion.
	/// </summary>
	public virtual string PostCode { get; set; }

	/// <summary>
	/// Initializes a parsed address.
	/// </summary>
	/// <param name="formattedAddress">The full formatted address.</param>
	/// <param name="coordinates">The geocoded coordinates.</param>
	/// <param name="provider">The provider name.</param>
	public ParsedAddress(string formattedAddress, Location coordinates, string provider)
		: base(formattedAddress, coordinates, provider) { }
}