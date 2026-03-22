using System.Collections;
using System.Reflection;
using System.Text.Json;
using Geocoding.Microsoft;
using Xunit;

namespace Geocoding.Tests;

[Collection("Settings")]
public class AzureMapsAsyncTest : AsyncGeocoderTest
{
    public AzureMapsAsyncTest(SettingsFixture settings)
        : base(settings)
    {
    }

    protected override IGeocoder CreateAsyncGeocoder()
    {
        SettingsFixture.SkipIfMissing(_settings.AzureMapsKey, nameof(SettingsFixture.AzureMapsKey));
        return new AzureMapsGeocoder(_settings.AzureMapsKey);
    }

    [Theory]
    [InlineData("1600 pennsylvania ave washington dc", EntityType.Address)]
    [InlineData("United States", EntityType.CountryRegion)]
    public async Task Geocode_AddressInput_ReturnsCorrectEntityType(string address, EntityType type)
    {
        // Arrange
        var geocoder = GetGeocoder<AzureMapsGeocoder>();

        // Act
        var results = (await geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken)).ToArray();

        // Assert
        Assert.Equal(type, results[0].Type);
    }

    [Fact]
    public void Constructor_EmptyApiKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new AzureMapsGeocoder(String.Empty));
    }

    [Fact]
    public void ParseResponse_SearchResultWithoutUsableFormattedAddress_SkipsEntry()
    {
        // Arrange
        var geocoder = new AzureMapsGeocoder("azure-key");

        const string json = """
                {
                    "results": [
                        {
                            "position": { "lat": 38.8976777, "lon": -77.036517 },
                            "address": {
                                "freeformAddress": "   ",
                                "municipality": "   ",
                                "country": "   "
                            }
                        }
                    ]
                }
                """;

        // Act
        var results = ParseResponse(geocoder, json);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void ParseResponse_ReverseResultWithoutUsableFormattedAddress_SkipsEntry()
    {
        // Arrange
        var geocoder = new AzureMapsGeocoder("azure-key");

        const string json = """
                {
                    "addresses": [
                        {
                            "position": "38.8976777,-77.036517",
                            "address": {
                                "freeformAddress": "   ",
                                "municipality": "   ",
                                "country": "   "
                            }
                        }
                    ]
                }
                """;

        // Act
        var results = ParseResponse(geocoder, json);

        // Assert
        Assert.Empty(results);
    }

    private static AzureMapsAddress[] ParseResponse(AzureMapsGeocoder geocoder, string json)
    {
        var responseType = typeof(AzureMapsGeocoder).GetNestedType("AzureSearchResponse", BindingFlags.NonPublic)!;
        var response = JsonSerializer.Deserialize(json, responseType);
        var parseMethod = typeof(AzureMapsGeocoder).GetMethod("ParseResponse", BindingFlags.Instance | BindingFlags.NonPublic)!;

        var results = (IEnumerable)parseMethod.Invoke(geocoder, [response!])!;
        return results.Cast<AzureMapsAddress>().ToArray();
    }
}
