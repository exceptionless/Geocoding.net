using Geocoding.Google;
using Xunit;

namespace Geocoding.Tests;

public class GoogleBusinessKeyTest
{
    [Fact]
    public void Constructor_NullClientId_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(delegate
        {
            new BusinessKey(null!, "signing-key");
        });
    }

    [Fact]
    public void Constructor_NullSigningKey_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(delegate
        {
            new BusinessKey("client-id", null!);
        });
    }

    [Fact]
    public void Constructor_WhitespaceValues_TrimsClientIdAndSigningKey()
    {
        // Act
        var key = new BusinessKey("  client-id    ", " signing-key   ");

        // Assert
        Assert.Equal("client-id", key.ClientId);
        Assert.Equal("signing-key", key.SigningKey);
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        // Arrange
        var key1 = new BusinessKey("client-id", "signing-key");
        var key2 = new BusinessKey("client-id", "signing-key");

        // Assert
        Assert.Equal(key1, key2);
        Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentClientIds_ReturnsFalse()
    {
        // Arrange
        var key1 = new BusinessKey("client-id1", "signing-key");
        var key2 = new BusinessKey("client-id2", "signing-key");

        // Assert
        Assert.NotEqual(key1, key2);
        Assert.NotEqual(key1.GetHashCode(), key2.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentSigningKeys_ReturnsFalse()
    {
        // Arrange
        var key1 = new BusinessKey("client-id", "signing-key1");
        var key2 = new BusinessKey("client-id", "signing-key2");

        // Assert
        Assert.NotEqual(key1, key2);
        Assert.NotEqual(key1.GetHashCode(), key2.GetHashCode());
    }

    [Fact]
    public void GenerateSignature_ValidUrl_ReturnsSignedUrl()
    {
        // Arrange
        var key = new BusinessKey("clientID", "vNIXE0xscrmjlyV-12Nj_BvUPaw=");

        // Act
        string signedUrl = key.GenerateSignature("http://maps.googleapis.com/maps/api/geocode/json?address=New+York&client=clientID");

        // Assert
        Assert.NotNull(signedUrl);
        Assert.Equal("http://maps.googleapis.com/maps/api/geocode/json?address=New+York&client=clientID&signature=chaRF2hTJKOScPr-RQCEhZbSzIE=", signedUrl);
    }

    [Theory]
    [InlineData("   Channel_1   ")]
    [InlineData(" channel-1")]
    [InlineData("CUSTOMER ")]
    public void Constructor_ChannelWithWhitespace_TrimsAndLowercases(string channel)
    {
        // Act
        var key = new BusinessKey("client-id", "signature", channel);

        // Assert
        Assert.Equal(channel.Trim().ToLower(), key.Channel);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("channel_1-2.")]
    public void Constructor_ValidChannelCharacters_DoesNotThrow(string? channel)
    {
        // Act & Assert
        new BusinessKey("client-id", "signature", channel);
    }

    [Theory]
    [InlineData("channel 1")]
    [InlineData("channel&1")]
    public void Constructor_SpecialCharactersInChannel_ThrowsArgumentException(string channel)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(delegate
        {
            new BusinessKey("client-id", "signature", channel);
        });
    }

    [Fact]
    public void ServiceUrl_WithBusinessKeyChannel_ContainsChannelName()
    {
        // Arrange
        var channel = "channel1";
        var key = new BusinessKey("client-id", "signature", channel);
        var geocoder = new GoogleGeocoder(key);

        // Assert
        Assert.Contains("channel=" + channel, geocoder.ServiceUrl);
    }

    [Fact]
    public void ServiceUrl_WithApiKey_DoesNotContainChannel()
    {
        // Arrange
        var geocoder = new GoogleGeocoder("apikey");

        // Assert
        Assert.DoesNotContain("channel=", geocoder.ServiceUrl);
    }

    [Fact]
    public void ServiceUrl_Default_DoesNotIncludeSensor()
    {
        // Arrange
        var geocoder = new GoogleGeocoder();

        // Assert
        Assert.DoesNotContain("sensor=", geocoder.ServiceUrl);
    }

    [Fact]
    public void ServiceUrl_ApiKeyIsNotSet_DoesNotIncludeKeyParameter()
    {
        // Arrange
        var geocoder = new GoogleGeocoder();

        // Act
        var serviceUrl = geocoder.ServiceUrl;

        // Assert
        Assert.DoesNotContain("&key=", serviceUrl);
    }

    [Fact]
    public void ServiceUrl_WithPostalCodeComponentFilter_ContainsPostalCodeFilter()
    {
        // Arrange
        var geocoder = new GoogleGeocoder("apikey")
        {
            ComponentFilters = new List<GoogleComponentFilter>
            {
                new(GoogleComponentFilterType.PostalCode, "NN14")
            }
        };

        // Assert
        Assert.Contains("components=postal_code:NN14", geocoder.ServiceUrl);
    }
}
