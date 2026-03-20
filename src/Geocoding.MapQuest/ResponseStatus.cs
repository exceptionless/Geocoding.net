namespace Geocoding.MapQuest
{
	/// <summary>
	/// Represents response status codes returned by MapQuest.
	/// </summary>
	public enum ResponseStatus : int
	{
		/// <summary>The Ok value.</summary>
		Ok = 0,
		/// <summary>The OkBatch value.</summary>
		OkBatch = 100,
		/// <summary>The ErrorInput value.</summary>
		ErrorInput = 400,
		/// <summary>The ErrorAccountKey value.</summary>
		ErrorAccountKey = 403,
		/// <summary>The ErrorUnknown value.</summary>
		ErrorUnknown = 500,
	}
}
