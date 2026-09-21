using System.Globalization;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Sufficit.Client.Controllers.Finance;
using Sufficit.Finance;
using Sufficit.Net.Http;
using Xunit;

namespace Sufficit.Client.IntegrationTests.Finance;

public sealed class RecentPaymentsRequestTests
{
    [Fact]
    public async Task SendsPagingAndEveryFilterUsingInvariantQueryValues()
    {
        using var handler = new CaptureHandler();
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://receipts.example") };
        var controller = new FinanceControllerSection(new AuthenticatedControllerBase(
            new StaticTokenProvider("fixture-token"), http, new JsonSerializerOptions(JsonSerializerDefaults.Web),
            NullLogger.Instance));
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
            var start = new DateTime(2026, 9, 15, 3, 0, 0, DateTimeKind.Utc);
            var end = start.AddDays(7).AddTicks(-1);
            var result = await controller.GetRecentPayments(new RecentPaymentSearchParameters
            {
                Start = start, End = end, MinimumValue = 125.50m,
                IncludeBankSlip = true, IncludeCard = false, IncludeMercadoPago = false,
                Paging = new PagingParameters { Position = 3, Size = 50 }
            });
            var query = System.Web.HttpUtility.ParseQueryString(handler.Uri!.Query);
            Assert.Equal("/finance/payment/recent", handler.Uri.AbsolutePath);
            Assert.Equal("3", query["Paging.Position"]);
            Assert.Equal("50", query["Paging.Size"]);
            Assert.Equal("125.50", query["MinimumValue"]);
            Assert.Equal("true", query["IncludeBankSlip"]);
            Assert.Equal("false", query["IncludeCard"]);
            Assert.Equal("false", query["IncludeMercadoPago"]);
            Assert.Equal(start, DateTime.Parse(query["Start"]!, null, DateTimeStyles.RoundtripKind));
            Assert.Equal(end, DateTime.Parse(query["End"]!, null, DateTimeStyles.RoundtripKind));
            Assert.Equal(528, result!.TotalCount);
            Assert.Equal(123456.78m, result.TotalValue);
        }
        finally { CultureInfo.CurrentCulture = previousCulture; }
    }

    private sealed class CaptureHandler : HttpMessageHandler
    {
        public Uri? Uri { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            Uri = request.RequestUri;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"items\":[],\"count\":0,\"totalCount\":528,\"totalValue\":123456.78}", System.Text.Encoding.UTF8, "application/json")
            });
        }
    }
}
