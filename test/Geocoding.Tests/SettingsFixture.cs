using Microsoft.Extensions.Configuration;
using Xunit;

namespace Geocoding.Tests;

public class SettingsFixture
{
	private readonly IConfigurationRoot _configuration;

	public SettingsFixture()
	{
		_configuration = new ConfigurationBuilder()
			.AddJsonFile("settings.json")
			.AddJsonFile("settings-override.json", optional: true)
			.Build();
	}

	public String YahooConsumerKey
	{
		get { return _configuration.GetValue<String>("yahooConsumerKey"); }
	}

	public String YahooConsumerSecret
	{
		get { return _configuration.GetValue<String>("yahooConsumerSecret"); }
	}

	public String BingMapsKey
	{
		get { return _configuration.GetValue<String>("bingMapsKey"); }
	}

	public String GoogleApiKey
	{
		get { return _configuration.GetValue<String>("googleApiKey"); }
	}

	public String MapQuestKey
	{
		get { return _configuration.GetValue<String>("mapQuestKey"); }
	}

	public String HereAppId
	{
		get { return _configuration.GetValue<String>("hereAppId"); }
	}

	public String HereAppCode
	{
		get { return _configuration.GetValue<String>("hereAppCode"); }
	}

	public static void SkipIfMissing(String value, String settingName)
	{
		if (String.IsNullOrWhiteSpace(value))
			Assert.Skip($"Integration test requires '{settingName}' in test/Geocoding.Tests/settings-override.json.");
	}
}

[CollectionDefinition("Settings")]
public class SettingsCollection : ICollectionFixture<SettingsFixture>
{
	// https://xunit.github.io/docs/shared-context.html
	// This class has no code, and is never created. Its purpose is simply
	// to be the place to apply [CollectionDefinition] and all the
	// ICollectionFixture<> interfaces.
}