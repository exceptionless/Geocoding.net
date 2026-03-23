using System.Collections;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using Geocoding.Here;
using Geocoding.Serialization;
using Xunit;

namespace Geocoding.Tests;

[Collection("Settings")]
public class HereAsyncGeocoderTest : AsyncGeocoderTest
{
    public HereAsyncGeocoderTest(SettingsFixture settings)
        : base(settings) { }

    protected override IGeocoder CreateAsyncGeocoder()
    {
        SettingsFixture.SkipIfMissing(_settings.HereApiKey, nameof(SettingsFixture.HereApiKey));
        return new HereGeocoder(_settings.HereApiKey);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public Task Geocode_BlankAddress_ThrowsArgumentException(string address)
    {
        // Arrange
        var geocoder = new HereGeocoder("here-api-key");

        // Act & Assert
        return Assert.ThrowsAsync<ArgumentException>(() => geocoder.GeocodeAsync(address, TestContext.Current.CancellationToken));
    }

    [Fact]
    public void ParseResponse_BlankFormattedAddress_SkipsEntry()
    {
        // Arrange
        var geocoder = new HereGeocoder("here-api-key");

        const string json = """
                {
                    "items": [
                        {
                            "title": "   ",
                            "address": {
                                "label": "   "
                            },
                            "position": {
                                "lat": 38.8976777,
                                "lng": -77.036517
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
    public async Task Geocode_HttpFailure_UsesTrimmedPreviewMessage()
    {
        // Arrange
        var body = new string('x', 300);
        var geocoder = new TestableHereGeocoder(new TestHttpMessageHandler((_, _) => TestHttpMessageHandler.CreateResponseAsync(HttpStatusCode.BadRequest, "Bad Request", body)));

        // Act
        var exception = await Assert.ThrowsAsync<HereGeocodingException>(() => geocoder.GeocodeAsync("1600 pennsylvania ave nw, washington dc", TestContext.Current.CancellationToken));

        // Assert
        Assert.Contains("HERE request failed (400 Bad Request).", exception.Message, StringComparison.Ordinal);
        Assert.Contains("Response preview:", exception.Message, StringComparison.Ordinal);
        Assert.DoesNotContain(body, exception.Message, StringComparison.Ordinal);
    }

    private static HereAddress[] ParseResponse(HereGeocoder geocoder, string json)
    {
        var responseType = typeof(HereGeocoder).GetNestedType("HereResponse", BindingFlags.NonPublic)!;
        var response = JsonSerializer.Deserialize(json, responseType, JsonExtensions.JsonOptions);
        var parseMethod = typeof(HereGeocoder).GetMethod("ParseResponse", BindingFlags.Instance | BindingFlags.NonPublic)!;

        var results = (IEnumerable)parseMethod.Invoke(geocoder, [response!])!;
        return results.Cast<HereAddress>().ToArray();
    }

    private sealed class TestableHereGeocoder : HereGeocoder
    {
        private readonly HttpMessageHandler _handler;

        public TestableHereGeocoder(HttpMessageHandler handler)
            : base("here-api-key")
        {
            _handler = handler;
        }

        protected override HttpClient BuildClient()
        {
            return new HttpClient(_handler, disposeHandler: false);
        }
    }

}
