using Geocoding.Extensions;
using Xunit;

namespace Geocoding.Tests;

public class TolerantStringEnumConverterTest
{
    [Fact]
    public void FromJson_UnknownStringForEnumWithUnknownMember_ReturnsUnknown()
    {
        // Arrange
        const string json = "{\"value\":\"something-new\"}";

        // Act
        var model = json.FromJson<EnumWithUnknownModel>();

        // Assert
        Assert.NotNull(model);
        Assert.Equal(EnumWithUnknown.Unknown, model!.Value);
    }

    [Fact]
    public void FromJson_UnknownNumberForEnumWithUnknownMember_ReturnsUnknown()
    {
        // Arrange
        const string json = "{\"value\":999}";

        // Act
        var model = json.FromJson<EnumWithUnknownModel>();

        // Assert
        Assert.NotNull(model);
        Assert.Equal(EnumWithUnknown.Unknown, model!.Value);
    }

    [Fact]
    public void FromJson_NullableEnumWithNullValue_ReturnsNull()
    {
        // Arrange
        const string json = "{\"value\":null}";

        // Act
        var model = json.FromJson<NullableEnumWithUnknownModel>();

        // Assert
        Assert.NotNull(model);
        Assert.Null(model!.Value);
    }

    [Fact]
    public void FromJson_UnknownStringWithoutUnknownMember_ReturnsDefaultValue()
    {
        // Arrange
        const string json = "{\"value\":\"something-new\"}";

        // Act
        var model = json.FromJson<EnumWithoutUnknownModel>();

        // Assert
        Assert.NotNull(model);
        Assert.Equal(EnumWithoutUnknown.First, model!.Value);
    }

    [Fact]
    public void FromJson_NumericStringForEnumWithUnknownMember_ReturnsUnknown()
    {
        // Arrange
        const string json = "{\"value\":\"999\"}";

        // Act
        var model = json.FromJson<EnumWithUnknownModel>();

        // Assert
        Assert.NotNull(model);
        Assert.Equal(EnumWithUnknown.Unknown, model!.Value);
    }

    [Fact]
    public void FromJson_NumericValueForByteEnum_ReturnsKnownValue()
    {
        // Arrange
        const string json = "{\"value\":1}";

        // Act
        var model = json.FromJson<ByteEnumWithUnknownModel>();

        // Assert
        Assert.NotNull(model);
        Assert.Equal(ByteEnumWithUnknown.Known, model!.Value);
    }

    [Fact]
    public void FromJson_UnknownNumericValueForByteEnum_ReturnsUnknown()
    {
        // Arrange
        const string json = "{\"value\":99}";

        // Act
        var model = json.FromJson<ByteEnumWithUnknownModel>();

        // Assert
        Assert.NotNull(model);
        Assert.Equal(ByteEnumWithUnknown.Unknown, model!.Value);
    }

    private sealed class EnumWithUnknownModel
    {
        public EnumWithUnknown Value { get; set; }
    }

    private sealed class ByteEnumWithUnknownModel
    {
        public ByteEnumWithUnknown Value { get; set; }
    }

    private sealed class NullableEnumWithUnknownModel
    {
        public EnumWithUnknown? Value { get; set; }
    }

    private sealed class EnumWithoutUnknownModel
    {
        public EnumWithoutUnknown Value { get; set; }
    }

    private enum EnumWithUnknown
    {
        Unknown = 0,
        Known = 1
    }

    private enum EnumWithoutUnknown
    {
        First = 0,
        Second = 1
    }

    private enum ByteEnumWithUnknown : byte
    {
        Unknown = 0,
        Known = 1
    }
}
