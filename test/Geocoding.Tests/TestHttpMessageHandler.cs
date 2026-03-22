using System.Net.Http;

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
}