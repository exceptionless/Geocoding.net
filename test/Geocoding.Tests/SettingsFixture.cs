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

    public String GoogleApiKey
    {
        get { return GetValue("googleApiKey"); }
    }

    public String AzureMapsKey
    {
        get { return GetValue("azureMapsKey"); }
    }

    public String BingMapsKey
    {
        get { return GetValue("bingMapsKey"); }
    }

    public String HereApiKey
    {
        get { return GetValue("hereApiKey"); }
    }

    public String MapQuestKey
    {
        get { return GetValue("mapQuestKey"); }
    }

    public String YahooConsumerKey
    {
        get { return GetValue("yahooConsumerKey"); }
    }

    public String YahooConsumerSecret
    {
        get { return GetValue("yahooConsumerSecret"); }
    }

    private String GetValue(string key)
    {
        String value = _configuration.GetValue<String>(key);
        return String.IsNullOrWhiteSpace(value) ? String.Empty : value;
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
