using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Sufficit.Client.Controllers.Gateway;
using Sufficit.Gateway.WhatsApp;
using Sufficit.Net.Http;
using Xunit;

namespace Sufficit.Client.IntegrationTests.Gateway;

public sealed class WhatsAppQuepasaRequestTests
{
    [Fact]
    public async Task StartingExistingSessionResumesWithoutCreatingNewPairing()
    {
        using var handler = new CaptureHandler();
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://gateway.example") };
        var controller = CreateController(http);
        const string sessionId = "5511999999999:5";

        await controller.Start(sessionId);

        Assert.Equal(HttpMethod.Post, handler.Method);
        Assert.Equal("/Gateway/WhatsApp/Quepasa/resume", handler.Uri!.AbsolutePath, ignoreCase: true);
        Assert.Equal(sessionId, System.Web.HttpUtility.ParseQueryString(handler.Uri.Query)["sessionid"]);
        Assert.Null(handler.Body);
    }

    [Fact]
    public async Task NewPairingStillStartsWithContextAndModeInJsonBody()
    {
        using var handler = new CaptureHandler();
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://gateway.example") };
        var contextId = Guid.NewGuid();

        await CreateController(http).Start(new WhatsAppQuepasaStartRequest { ContextId = contextId, Mode = "qrcode" });

        Assert.Equal(HttpMethod.Post, handler.Method);
        Assert.Equal("/Gateway/WhatsApp/Quepasa/start", handler.Uri!.AbsolutePath, ignoreCase: true);
        var body = JsonSerializer.Deserialize<WhatsAppQuepasaStartRequest>(handler.Body!, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.Equal(contextId, body!.ContextId);
        Assert.Equal("qrcode", body.Mode);
    }

    private static WhatsAppQuepasaControllerSection CreateController(HttpClient http) => new(
        new AuthenticatedControllerBase(new StaticTokenProvider("fixture-token"), http,
            new JsonSerializerOptions(JsonSerializerDefaults.Web), NullLogger.Instance));

    private sealed class CaptureHandler : HttpMessageHandler
    {
        public Uri? Uri { get; private set; }
        public HttpMethod? Method { get; private set; }
        public string? Body { get; private set; }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            Uri = request.RequestUri;
            Method = request.Method;
            Body = request.Content is null ? null : await request.Content.ReadAsStringAsync(token);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };
        }
    }
}
