using System.Net;
using System.Net.Http;
using System.Text;

namespace Geocoding.Tests;

internal sealed class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _sendAsync;

    public TestHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync)
    {
        _sendAsync = sendAsync;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return _sendAsync(request, cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateResponseAsync(HttpStatusCode statusCode, string? reasonPhrase = null, string? body = null)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            ReasonPhrase = reasonPhrase
        };

        if (!String.IsNullOrWhiteSpace(body))
            response.Content = new StringContent(body, Encoding.UTF8, "text/plain");

        return Task.FromResult(response);
    }
}