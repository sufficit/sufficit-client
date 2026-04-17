using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.EndPoints.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sufficit.Client.IntegrationTests.Telephony
{
    [Trait("Category", "Integration")]
    public sealed class OrganizationIntegrationTests
    {
        private readonly ITestOutputHelper _output;
        private readonly APIClientService _apiClient;
        private readonly Guid _knownContextId;

        public OrganizationIntegrationTests(ITestOutputHelper output)
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

            var knownContext = configuration["Sufficit:TestData:Telephony:Organization:KnownContextId"]
                ?? throw new InvalidOperationException("Known telephony organization context not configured in appsettings.json");

            _knownContextId = Guid.Parse(knownContext);

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
        public async Task GetOrganization_ReturnsOrganizationForKnownContext()
        {
            var result = await _apiClient.Telephony.GetOrganization(_knownContextId, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(_knownContextId, result!.ContextId);

            _output.WriteLine($"Organization returned for context {_knownContextId}");
            _output.WriteLine($"Legacy preview pending total: {result.LegacyPurgePreview?.PendingTotal ?? 0}");
        }

        [Fact]
        [Trait("Database", "ReadOnly")]
        public async Task PurgeOrganization_TestingMode_DoesNotReturnLegacyPolls()
        {
            var result = (await _apiClient.Telephony.PurgeOrganization(_knownContextId, true, CancellationToken.None)).ToList();

            Assert.NotNull(result);
            Assert.DoesNotContain(result, item => string.Equals(item.Type, "LegacyPolls", StringComparison.OrdinalIgnoreCase));
            Assert.All(result, item =>
            {
                Assert.False(string.IsNullOrWhiteSpace(item.Type));
                Assert.True(item.Count > 0);
            });

            _output.WriteLine($"Purge preview returned {result.Count} item(s) for context {_knownContextId}");
        }
    }
}