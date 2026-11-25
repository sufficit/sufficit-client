using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Sufficit.Client.Controllers.Telephony;
using Sufficit.Net.Http;
using Sufficit.Telephony;
using Xunit;

namespace Sufficit.Client.IntegrationTests.Telephony;

public class DIDRoutingContractTests
{
    [Fact]
    public async Task Snapshot_round_trips_and_conflict_reaches_the_caller()
    {
        var expected = new QueueDidRouteSnapshot(Guid.NewGuid(), Guid.NewGuid(), "route", Guid.NewGuid(), "endpoint", null, true);
        var queue = Guid.NewGuid();
        using var handler = new CaptureHandler();
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://sdk.test") };
        var client = new TelephonyDIDControllerSection(new AuthenticatedControllerBase(new StaticTokenProvider("fixture"),
            http, new JsonSerializerOptions(JsonSerializerDefaults.Web), NullLogger.Instance));
        handler.Response = new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(expected) };
        var actual = await client.GetDestination(expected.DidId, expected.ContextId);
        Assert.Equal(expected, actual);
        Assert.Equal(("GET", $"/telephony/did/destination?id={expected.DidId}&contextId={expected.ContextId}"), handler.Last);

        handler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);
        await client.RemoveDestination(actual!);
        Assert.Equal(("DELETE", "/telephony/did/destination"), handler.Last);
        Assert.Equal(expected, handler.Body.Deserialize<QueueDidRouteSnapshot>(JsonSerializerOptions.Web));

        var change = new QueueDidRouteChange(expected.ContextId, queue, true, expected.ExpectedAsterisk,
            expected.ExpectedDestinationId, expected.ExpectedTypeName, expected.ExpectedAnswer, expected.ExpectedBilled);
        handler.Response = new HttpResponseMessage(HttpStatusCode.Conflict);
        var error = await Assert.ThrowsAsync<HttpRequestException>(() => client.SetQueueDestination(expected.DidId, change));
        Assert.Equal(HttpStatusCode.Conflict, error.StatusCode);
        Assert.Equal(("POST", $"/telephony/did/queueDestination?id={expected.DidId}"), handler.Last);
        Assert.Equal(change, handler.Body.Deserialize<QueueDidRouteChange>(JsonSerializerOptions.Web));

        handler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);
        await client.Activity("1234", 0);
        Assert.Equal(("GET", "/telephony/did/activity?extension=1234&days=0"), handler.Last);
        handler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);
        await client.Activity("1234");
        Assert.Equal(("GET", "/telephony/did/activity?extension=1234"), handler.Last);
    }

    private sealed class CaptureHandler : HttpMessageHandler
    {
        public (string, string) Last { get; private set; }
        public JsonElement Body { get; private set; }
        public HttpResponseMessage Response { get; set; } = null!;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Last = (request.Method.Method, request.RequestUri!.PathAndQuery);
            if (request.Content != null) Body = await request.Content.ReadFromJsonAsync<JsonElement>(ct);
            return Response;
        }
    }
}
