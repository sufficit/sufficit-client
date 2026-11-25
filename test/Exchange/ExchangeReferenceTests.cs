using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Sufficit.Client.Controllers;
using Sufficit.Exchange;
using Sufficit.Net.Http;
using Xunit;

namespace Sufficit.Client.IntegrationTests.Exchange;

public sealed class ExchangeReferenceTests
{
    [Fact]
    public async Task Reference_queries_use_the_message_search_contract_and_read_receipt_query()
    {
        var id = Guid.NewGuid();
        using var handler = new ReferenceHandler(id);
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://sdk.test") };
        var sdk = new ExchangeControllerSection(new AuthenticatedControllerBase(new StaticTokenProvider("fixture"),
            http, JsonSerializerOptions.Web, NullLogger.Instance));
        Assert.Empty(await sdk.Messages.GetByReferenceId(id, default));
        var receipts = new List<IReadReceipt>();
        await foreach (var receipt in sdk.Reads.GetByReferenceId(id, default)) receipts.Add(receipt);
        Assert.Equal("opened", Assert.Single(receipts).Description);
        Assert.Equal(2026, receipts[0].Timestamp.Year);
    }

    private sealed class ReferenceHandler(Guid reference) : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var uri = request.RequestUri!;
            string result;
            if (request.Method == HttpMethod.Post)
            {
                Assert.Equal("/exchange/messages", uri.AbsolutePath);
                var parameters = JsonSerializer.Deserialize<MessageDetailsSearchParameters>(
                    await request.Content!.ReadAsStringAsync(ct), JsonSerializerOptions.Web);
                Assert.Equal(reference, parameters!.ReferenceId);
                result = "[]";
            }
            else
            {
                Assert.Equal(HttpMethod.Get, request.Method);
                Assert.Equal($"/exchange/messages/reads/ByReferenceId?id={reference:D}", uri.PathAndQuery);
                result = """[{"timestamp":"2026-01-01T00:00:00Z","description":"opened"}]""";
            }
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(result, Encoding.UTF8, "application/json") };
        }
    }
}
