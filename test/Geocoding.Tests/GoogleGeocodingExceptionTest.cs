using Geocoding.Google;
using Xunit;

namespace Geocoding.Tests;

public class GoogleGeocodingExceptionTest
{
    [Fact]
    public void Constructor_WithProviderMessage_PreservesStatusAndMessage()
    {
        // Act
        var exception = new GoogleGeocodingException(GoogleStatus.RequestDenied, "This API is not activated on your API project.");

        // Assert
        Assert.Equal(GoogleStatus.RequestDenied, exception.Status);
        Assert.Equal("This API is not activated on your API project.", exception.ProviderMessage);
        Assert.Contains("RequestDenied", exception.Message);
        Assert.Contains("This API is not activated on your API project.", exception.Message);
    }

    [Fact]
    public void Constructor_WithoutProviderMessage_LeavesProviderMessageNull()
    {
        // Act
        var exception = new GoogleGeocodingException(GoogleStatus.OverQueryLimit);

        // Assert
        Assert.Equal(GoogleStatus.OverQueryLimit, exception.Status);
        Assert.Null(exception.ProviderMessage);
        Assert.Contains("OverQueryLimit", exception.Message);
    }
}