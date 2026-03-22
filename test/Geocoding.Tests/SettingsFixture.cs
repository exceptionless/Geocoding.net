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
            .AddEnvironmentVariables("GEOCODING_")
            .Build();
    }

    public String GoogleApiKey
    {
        get { return GetValue("Providers:Google:ApiKey", "googleApiKey"); }
    }

    public String AzureMapsKey
    {
        get { return GetValue("Providers:Azure:ApiKey", "azureMapsKey"); }
    }

    public String BingMapsKey
    {
        get { return GetValue("Providers:Bing:ApiKey", "bingMapsKey"); }
    }

    public String HereApiKey
    {
        get { return GetValue("Providers:Here:ApiKey", "hereApiKey"); }
    }

    public String MapQuestKey
    {
        get { return GetValue("Providers:MapQuest:ApiKey", "mapQuestKey"); }
    }

    public String YahooConsumerKey
    {
        get { return GetValue("Providers:Yahoo:ConsumerKey", "yahooConsumerKey"); }
    }

    public String YahooConsumerSecret
    {
        get { return GetValue("Providers:Yahoo:ConsumerSecret", "yahooConsumerSecret"); }
    }

    private String GetValue(params string[] keys)
    {
        foreach (string key in keys)
        {
            String? value = _configuration[key];
            if (!String.IsNullOrWhiteSpace(value))
                return value;
        }

        return String.Empty;
    }

    public static void SkipIfMissing(String value, String settingName)
    {
        if (String.IsNullOrWhiteSpace(value))
            Assert.Skip($"Integration test requires '{settingName}' — set it in test/Geocoding.Tests/settings-override.json using the Providers section or via a GEOCODING_ environment variable.");
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
