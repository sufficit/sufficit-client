using Microsoft.Extensions.Logging.Abstractions;
using Sufficit.Client.Controllers.Telephony;
using Sufficit.Identity;
using Sufficit.Net.Http;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Sufficit.Client.Tests.Telephony;

public sealed class ChannelTagsRequestTests
{
    private static readonly Guid Context = Guid.NewGuid();
    private const string Protocol = "1795778413.1";

    [Fact]
    public async Task ReadsActiveHistoryInOneAuthenticatedRequest()
    {
        var start = new DateTimeOffset(2026, 11, 27, 10, 0, 0, TimeSpan.FromHours(-3));
        var count = 0;
        var client = Client(async request =>
        {
            count++;
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal($"/Telephony/Call/ChannelTags/History?contextid={Context:D}", request.RequestUri!.PathAndQuery);
            Assert.Equal("fixture", request.Headers.Authorization?.Parameter);
            var rows = await request.Content!.ReadFromJsonAsync<CallHistoryTagTarget[]>();
            Assert.Equal(start, rows![0].Start);
            Assert.Equal(Protocol, rows[0].LinkedId);
            Assert.Equal(2, rows.Length);
            return new(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new[] { new CallHistoryTags { LinkedId = Protocol, Tags = ["spam"] } })
            };
        });

        var tags = await client.ReadHistoryTags(Context,
            [new() { LinkedId = Protocol, Start = start }, new() { LinkedId = "1795778414.1", Start = start }], default);
        Assert.Equal("spam", Assert.Single(Assert.Single(tags!).Tags));
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task SingleCallReadAndWritePreserveAssignmentAndNullableReason()
    {
        var id = Guid.NewGuid();
        var client = Client(async request =>
        {
            Assert.Equal($"/Telephony/Call/ChannelTags?contextid={Context:D}&linkedid={Protocol}", request.RequestUri!.PathAndQuery);
            if (request.Method == HttpMethod.Get)
                return new(HttpStatusCode.NotFound);
            Assert.Equal(HttpMethod.Post, request.Method);
            var draft = await request.Content!.ReadFromJsonAsync<CallChannelTagDraft>();
            Assert.Equal(id, draft!.Id);
            Assert.Null(draft.Reason);
            return new(HttpStatusCode.OK) { Content = JsonContent.Create(new CallChannelTag { Id = id, Tag = "spam" }) };
        });

        Assert.Null(await client.ReadChannelTags(Context, Protocol, default));
        Assert.Equal(id, (await client.AddChannelTag(Context, Protocol,
            new() { Id = id, Server = "google", ChannelId = Protocol, Tag = "spam" }, default)).Id);
    }

    private static TelephonyCallControllerSection Client(Func<HttpRequestMessage, Task<HttpResponseMessage>> handle)
        => new(new AuthenticatedControllerBase(new Token(),
            new HttpClient(new Handler(handle)) { BaseAddress = new Uri("https://fixture.invalid/") },
            new JsonSerializerOptions(JsonSerializerDefaults.Web), NullLogger.Instance));

    private sealed class Handler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handle) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) => handle(request);
    }

    private sealed class Token : ITokenProvider
    {
        public ValueTask<string?> GetTokenAsync() => ValueTask.FromResult<string?>("fixture");
    }
}
