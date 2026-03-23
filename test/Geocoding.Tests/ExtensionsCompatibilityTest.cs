using System.Text.Json;
using Xunit;

namespace Geocoding.Tests;

public class ExtensionsCompatibilityTest
{
    [Fact]
    public void IsNullOrEmpty_LegacyShim_ReturnsExpectedValues()
    {
        ICollection<string>? nullCollection = null;

        Assert.True(global::Geocoding.Extensions.IsNullOrEmpty(nullCollection));
        Assert.True(global::Geocoding.Extensions.IsNullOrEmpty(Array.Empty<string>()));
        Assert.False(global::Geocoding.Extensions.IsNullOrEmpty(new[] { "value" }));
    }

    [Fact]
    public void ForEach_LegacyShim_ExecutesAction()
    {
        var values = new List<int>();

        global::Geocoding.Extensions.ForEach(new[] { 1, 2, 3 }, values.Add);

        Assert.Equal(new[] { 1, 2, 3 }, values);
    }

    [Fact]
    public void JsonHelpers_LegacyShim_RoundTripValue()
    {
        var payload = new CompatibilityPayload { Value = "hello" };

        var json = global::Geocoding.Extensions.ToJSON(payload);
        var roundTrip = global::Geocoding.Extensions.FromJSON<CompatibilityPayload>(json);

        Assert.Equal("hello", roundTrip!.Value);
    }

    [Fact]
    public void JsonOptions_LegacyShim_UsesSharedOptions()
    {
        const string json = "{\"value\":\"999\"}";

        var model = JsonSerializer.Deserialize<CompatibilityEnumPayload>(json, global::Geocoding.Extensions.JsonOptions);

        Assert.NotNull(model);
        Assert.Equal(CompatibilityEnum.Unknown, model!.Value);
    }

    private sealed class CompatibilityPayload
    {
        public string? Value { get; set; }
    }

    private sealed class CompatibilityEnumPayload
    {
        public CompatibilityEnum Value { get; set; }
    }

    private enum CompatibilityEnum
    {
        Unknown = 0,
        Known = 1
    }
}