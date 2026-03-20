using Xunit;

namespace Geocoding.Tests;

public abstract class BatchGeocoderTest
{
	private readonly IBatchGeocoder _batchGeocoder;
	protected readonly SettingsFixture _settings;

	public BatchGeocoderTest(SettingsFixture settings)
	{
		//Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-us");

		_settings = settings;

		_batchGeocoder = CreateBatchGeocoder();
	}

	protected abstract IBatchGeocoder CreateBatchGeocoder();

	[Theory]
	[MemberData(nameof(BatchGeoCodeData))]
	public virtual async Task CanGeoCodeAddress(String[] addresses)
	{
		Assert.NotEmpty(addresses);

		var results = await _batchGeocoder.GeocodeAsync(addresses, TestContext.Current.CancellationToken);
		Assert.NotEmpty(results);
		Assert.Equal(addresses.Length, results.Count());

		var addressSet = new HashSet<String>(addresses);
		Assert.Equal(addressSet.Count, results.Count());

		foreach (ResultItem resultItem in results)
		{
			Assert.NotNull(resultItem);
			Assert.NotNull(resultItem.Request);
			Assert.NotNull(resultItem.Response);

			Assert.Contains(resultItem.Request.FormattedAddress, addressSet);

			var responseAddresses = resultItem.Response.ToArray();
			Assert.NotEmpty(responseAddresses);

			addressSet.Remove(resultItem.Request.FormattedAddress);
		}
		Assert.Empty(addressSet);
	}

	public static IEnumerable<object[]> BatchGeoCodeData
	{
		get
		{
			yield return new object[]
			{
				new String[]
				{
					"1600 pennsylvania ave nw, washington dc",
					"1460 4th Street Ste 304, Santa Monica CA 90401",
				},
			};
		}
	}

}