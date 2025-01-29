using System.Net;

namespace Tests.Common.Mocks;

public sealed class MockHttpMessageHandler : HttpMessageHandler
{
    public HttpRequestMessage? CapturedRequest { get; private set; }
    public Func<HttpResponseMessage> HttpResponseMessageFactory { get; set; } = () => new HttpResponseMessage(HttpStatusCode.OK);

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        CapturedRequest = request;

        cancellationToken.ThrowIfCancellationRequested();

        var response = HttpResponseMessageFactory();
        response.RequestMessage = request;
        return Task.FromResult(response);
    }
}
