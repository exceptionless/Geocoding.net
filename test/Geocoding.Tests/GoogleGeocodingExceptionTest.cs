using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Geocoding.Google;
using Xunit;

namespace Geocoding.Tests;

public class GoogleGeocodingExceptionTest
{
    [Fact]
    public async Task ProcessWebResponse_RequestDenied_PreservesProviderMessage()
    {
        // Arrange
        var geocoder = new GoogleGeocoder();
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("<GeocodeResponse><status>REQUEST_DENIED</status><error_message>This API is not activated on your API project.</error_message></GeocodeResponse>", Encoding.UTF8, "application/xml")
        };
        MethodInfo method = typeof(GoogleGeocoder).GetMethod("ProcessWebResponse", BindingFlags.Instance | BindingFlags.NonPublic)!;

        // Act
        var task = (Task<IEnumerable<GoogleAddress>>)method.Invoke(geocoder, new object?[] { response })!;
        var exception = await Assert.ThrowsAsync<GoogleGeocodingException>(async () => await task);

        // Assert
        Assert.Equal(GoogleStatus.RequestDenied, exception.Status);
        Assert.Equal("This API is not activated on your API project.", exception.ProviderMessage);
        Assert.Contains("This API is not activated on your API project.", exception.Message);
    }

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