using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.EndPoints.Configuration;
using Sufficit.Telephony;
using Sufficit.Telephony.CallForward;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sufficit.Client.IntegrationTests.Telephony
{
    [Trait("Category", "Integration")]
    public sealed class CallForwardIntegrationTests
    {
        private const uint SearchLimit = 20;

        private readonly ITestOutputHelper _output;
        private readonly APIClientService _apiClient;
        private readonly Guid _contextId;
        private readonly string? _externalDestinationE164;
        private readonly string? _internalDestinationAsterisk;

        public CallForwardIntegrationTests(ITestOutputHelper output)
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

            var knownOrganizationContext = configuration["Sufficit:TestData:Telephony:Organization:KnownContextId"]
                ?? throw new InvalidOperationException("Known telephony organization context not configured in appsettings.json");

            var configuredContext = configuration["Sufficit:TestData:Telephony:CallForward:ContextId"];
            _contextId = Guid.Parse(string.IsNullOrWhiteSpace(configuredContext) ? knownOrganizationContext : configuredContext);

            _externalDestinationE164 = configuration["Sufficit:TestData:Telephony:CallForward:ExternalDestinationE164"];
            _internalDestinationAsterisk = configuration["Sufficit:TestData:Telephony:CallForward:InternalDestinationAsterisk"];

            var timeout = uint.TryParse(configuration["Sufficit:EndPoints:TimeOut"], out var t) ? t : 30u;

            var options = new EndPointsAPIOptions
            {
                BaseAddress = endpointsUrl,
                TimeOut = timeout
            };

            _apiClient = new APIClientService(
                Options.Create(options),
                new StaticTokenProvider(token),
                NullLogger<APIClientService>.Instance);
        }

        [Fact]
        public async Task ExternalCallForward_CreateSearchDelete_ShouldPersistWithConfiguredCellphone()
        {
            if (string.IsNullOrWhiteSpace(_externalDestinationE164))
            {
                _output.WriteLine("Skipping external CallForward integration test. Configure Sufficit:TestData:Telephony:CallForward:ExternalDestinationE164 with your cellphone in E.164 format.");
                return;
            }

            var callerIdOptions = (await _apiClient.Telephony.CallForward.GetCallerIdOptions(_contextId, CancellationToken.None)).ToList();
            Assert.NotEmpty(callerIdOptions);

            var selectedCallerId = callerIdOptions[0].Number;
            Assert.False(string.IsNullOrWhiteSpace(selectedCallerId));

            var item = new CallForwardApplication
            {
                Id = Guid.NewGuid(),
                ContextId = _contextId,
                Title = BuildUniqueTitle("it-ext"),
                Description = "Integration test - external forward",
                Destination = _externalDestinationE164,
                Timeout = 30,
                OutboundCallerIdNumber = selectedCallerId,
#pragma warning disable CS0618
                Masked = false,
#pragma warning restore CS0618
            };

            _output.WriteLine($"Creating external CallForward in context {_contextId} to destination {item.Destination} with outbound caller id {selectedCallerId}");

            await _apiClient.Telephony.CallForward.AddOrUpdate(item, CancellationToken.None);

            try
            {
                var saved = await FindById(item.Id, CancellationToken.None);
                Assert.NotNull(saved);
                Assert.Equal(item.ContextId, saved!.ContextId);
                Assert.Equal(item.Title, saved.Title);
                Assert.Equal(item.Destination, saved.Destination);
                Assert.Equal(item.OutboundCallerIdNumber, saved.OutboundCallerIdNumber);
                Assert.Null(saved.Deleted);
            }
            finally
            {
                await _apiClient.Telephony.CallForward.Delete(item.Id, CancellationToken.None);

                var afterDelete = await FindById(item.Id, CancellationToken.None, includeDeleted: true);
                Assert.NotNull(afterDelete);
                Assert.NotNull(afterDelete!.Deleted);
            }
        }

        [Fact]
        public async Task InternalCallForward_CreateSearchDelete_ShouldPersistWithInternalDestination()
        {
            var internalAsterisk = await ResolveInternalDestinationAsterisk(CancellationToken.None);
            if (string.IsNullOrWhiteSpace(internalAsterisk))
            {
                _output.WriteLine("Skipping internal CallForward integration test. No internal destination was found for the configured context.");
                return;
            }

            var item = new CallForwardApplication
            {
                Id = Guid.NewGuid(),
                ContextId = _contextId,
                Title = BuildUniqueTitle("it-int"),
                Description = "Integration test - internal forward",
                Destination = internalAsterisk,
                Timeout = 30,
                OutboundCallerIdNumber = null,
#pragma warning disable CS0618
                Masked = false,
#pragma warning restore CS0618
            };

            _output.WriteLine($"Creating internal CallForward in context {_contextId} to destination {item.Destination}");

            await _apiClient.Telephony.CallForward.AddOrUpdate(item, CancellationToken.None);

            try
            {
                var saved = await FindById(item.Id, CancellationToken.None);
                Assert.NotNull(saved);
                Assert.Equal(item.ContextId, saved!.ContextId);
                Assert.Equal(item.Title, saved.Title);
                Assert.Equal(item.Destination, saved.Destination);
                Assert.Null(saved.Deleted);
            }
            finally
            {
                await _apiClient.Telephony.CallForward.Delete(item.Id, CancellationToken.None);

                var afterDelete = await FindById(item.Id, CancellationToken.None, includeDeleted: true);
                Assert.NotNull(afterDelete);
                Assert.NotNull(afterDelete!.Deleted);
            }
        }

        private async Task<CallForwardApplication?> FindById(Guid id, CancellationToken cancellationToken, bool includeDeleted = false)
        {
            var parameters = new CallForwardSearchParameters
            {
                CallForwardId = id,
                Limit = 1,
                Deleted = includeDeleted ? null : false,
            };

            var items = await _apiClient.Telephony.CallForward.Search(parameters, cancellationToken);
            return items.FirstOrDefault();
        }

        private async Task<string?> ResolveInternalDestinationAsterisk(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(_internalDestinationAsterisk))
                return _internalDestinationAsterisk;

            var destinations = await _apiClient.Telephony.Destination.Search(new DestinationSearchParameters
            {
                ContextId = _contextId,
                Limit = SearchLimit,
            }, cancellationToken);

            var selected = destinations.FirstOrDefault(d =>
                !string.IsNullOrWhiteSpace(d.Asterisk) &&
                !d.Asterisk.StartsWith(CallForwardApplication.ASTERISKCONTEXT, StringComparison.OrdinalIgnoreCase));

            if (selected != null)
                _output.WriteLine($"Resolved internal destination from API: {selected.TypeName} -> {selected.Asterisk}");

            return selected?.Asterisk;
        }

        private static string BuildUniqueTitle(string prefix)
            => $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}";
    }
}
