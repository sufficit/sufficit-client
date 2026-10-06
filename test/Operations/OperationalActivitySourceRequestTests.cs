using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Sufficit.Client.Controllers;
using Sufficit.Net.Http;
using Sufficit.Operations;
using Xunit;

namespace Sufficit.Client.IntegrationTests.Operations;

public sealed class OperationalActivitySourceRequestTests
{
    private static OperationalActivityRequestedEvent Source() => new()
    {
        EventId = Guid.NewGuid(), ContextId = Guid.NewGuid(), CorrelationId = Guid.NewGuid(),
        OccurredAtUtc = new DateTime(2026, 10, 5, 12, 0, 0, DateTimeKind.Utc),
        Reason = "Requested follow-up", ActivityType = "customer-follow-up", Title = "Review requested work"
    };

    [Fact]
    public async Task AuthenticatedEventRetriesPreserveTheOriginalPayloadAndIdentity()
    {
        using var handler = new CaptureHandler();
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://activities.example") };
        var client = new OperationalActivitiesControllerSection(new AuthenticatedControllerBase(
            new StaticTokenProvider("fixture-token"), http, new JsonSerializerOptions(JsonSerializerDefaults.Web), NullLogger.Instance));
        var source = Source();
        await client.OpenFromEvent(source, default); await client.OpenFromEvent(source, default);
        Assert.Equal(2, handler.Bodies.Count);
        Assert.Equal(handler.Bodies[0], handler.Bodies[1]);
        Assert.Equal("/Operations/Activities/SourceEvents", handler.Path);
        Assert.Equal("Bearer fixture-token", handler.Authorization);
        using var json = JsonDocument.Parse(handler.Bodies[0]);
        Assert.Equal(source.EventId, json.RootElement.GetProperty("eventId").GetGuid());
        Assert.Equal(source.ContextId, json.RootElement.GetProperty("contextId").GetGuid());
        Assert.Equal(source.OccurredAtUtc, json.RootElement.GetProperty("occurredAtUtc").GetDateTime());
        Assert.False(json.RootElement.TryGetProperty("actorId", out _));
        Assert.False(json.RootElement.TryGetProperty("expectedRevision", out _));
    }

    [Fact]
    public async Task InvalidSourceDoesNotSendAnHttpRequest()
    {
        using var handler = new CaptureHandler();
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://activities.example") };
        var client = new OperationalActivitiesControllerSection(new AuthenticatedControllerBase(
            new StaticTokenProvider("fixture-token"), http, new JsonSerializerOptions(JsonSerializerDefaults.Web), NullLogger.Instance));
        var source = Source(); source.EventId = Guid.Empty;
        await Assert.ThrowsAsync<ArgumentException>(() => client.OpenFromEvent(source, default));
        Assert.Empty(handler.Bodies);
    }

    private sealed class CaptureHandler : HttpMessageHandler
    {
        public List<string> Bodies { get; } = [];
        public string? Path { get; private set; }
        public string? Authorization { get; private set; }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            Path = request.RequestUri!.AbsolutePath; Authorization = request.Headers.Authorization?.ToString();
            Bodies.Add(await request.Content!.ReadAsStringAsync(token));
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"actorId\":\"00000000-0000-0000-0000-000000000001\",\"reason\":\"Recorded source\",\"after\":{}}", System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}
