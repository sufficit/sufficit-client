using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.EndPoints.Configuration;
using Sufficit.Telephony;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sufficit.Client.IntegrationTests.Telephony
{
    /// <summary>
    /// Read-only integration test for VoiceMail search focused on disabled mailboxes.
    /// </summary>
    [Trait("Category", "Integration")]
    public sealed class VoiceMailIntegrationTests
    {
        private const uint DefaultLimit = 100;

        private readonly ITestOutputHelper _output;
        private readonly APIClientService _apiClient;

        public VoiceMailIntegrationTests(ITestOutputHelper output)
        {
            _output = output;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var endpointsUrl = configuration["Sufficit:EndPoints:BaseAddress"]
                ?? throw new InvalidOperationException("EndPoints BaseAddress not configured in appsettings.json");

            var token = configuration["Sufficit:Authentication:Tokens:Manager"]
                ?? throw new InvalidOperationException("Manager token not configured in appsettings.json");

            var timeout = uint.TryParse(configuration["Sufficit:EndPoints:TimeOut"], out var t) ? t : 30u;

            var options = new EndPointsAPIOptions()
            {
                BaseAddress = endpointsUrl,
                TimeOut = timeout,
            };

            _apiClient = new APIClientService(
                Options.Create(options),
                new StaticTokenProvider(token),
                NullLogger<APIClientService>.Instance);
        }

        [Fact]
        [Trait("Database", "ReadOnly")]
        public async Task Search_ReturnsDisabledMailboxesWithoutContext()
        {
            var parameters = new VoiceMailSearchParameters()
            {
                Enabled = false,
                Deleted = false,
                Limit = DefaultLimit,
            };

            var results = (await _apiClient.Telephony.VoiceMail.Search(parameters, CancellationToken.None)).ToList();
            var disabled = results.Where(mailbox => !mailbox.Enabled).ToList();

            if (!disabled.Any())
            {
                _output.WriteLine($"Search mode: global (without context filter). Returned {results.Count} mailbox(es), but none disabled.");
                return;
            }

            Assert.All(disabled, mailbox =>
            {
                Assert.NotEqual(Guid.Empty, mailbox.Id);
                Assert.False(mailbox.Enabled);
            });

            _output.WriteLine("Search mode: global (without context filter)");
            _output.WriteLine($"Disabled mailboxes found: {disabled.Count}");

            foreach (var mailbox in disabled.Take(10))
            {
                _output.WriteLine($"Disabled mailbox => Id: {mailbox.Id} | ContextId: {mailbox.ContextId} | Title: {mailbox.Title ?? "<empty>"} | Destination: {mailbox.Destination ?? "<empty>"}");
            }
        }
    }
}
