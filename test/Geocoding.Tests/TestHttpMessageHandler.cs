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

    public static HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string? reasonPhrase = null, string? body = null)
    {
        return new HttpResponseMessage(statusCode)
        {
            ReasonPhrase = reasonPhrase,
            Content = String.IsNullOrWhiteSpace(body) ? null : new StringContent(body, Encoding.UTF8, "text/plain")
        };
    }

    public static Task<HttpResponseMessage> CreateResponseAsync(HttpStatusCode statusCode, string? reasonPhrase = null, string? body = null)
    {
        return Task.FromResult(CreateResponse(statusCode, reasonPhrase, body));
    }
}