using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sufficit.EndPoints.Configuration;
using Sufficit.Telephony;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sufficit.Client.IntegrationTests.Telephony.Routes
{
    /// <summary>
    /// Integration tests for the admin-only routes registration status endpoint.
    /// These tests are intentionally configurable because the known route id lives
    /// in a specific environment and the endpoint requires an administrator token.
    /// </summary>
    [Trait("Category", "Integration")]
    public sealed class RoutesRegistrationStatusesIntegrationTests
    {
        private readonly ITestOutputHelper _output;
        private readonly APIClientService? _adminClient;
        private readonly Guid? _knownInterconnectionId;
        private readonly Guid? _knownRegistrationId;
        private readonly string? _knownRuntimeId;
        private readonly string? _expectedNode;

        public RoutesRegistrationStatusesIntegrationTests(ITestOutputHelper output)
        {
            _output = output;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var endpointsUrl = configuration["Sufficit:EndPoints:BaseAddress"]
                ?? throw new InvalidOperationException("EndPoints BaseAddress not configured in appsettings.json");

            var timeout = uint.TryParse(configuration["Sufficit:EndPoints:TimeOut"], out var parsedTimeout) ? parsedTimeout : 30u;
            var adminToken = configuration["Sufficit:Authentication:Tokens:Administrator"]
                ?? configuration["Sufficit:Authentication:Tokens:Admin"];

            var interconnectionRaw = configuration["Sufficit:TestData:Telephony:Routes:KnownInterconnectionId"];
            var registrationRaw = configuration["Sufficit:TestData:Telephony:Routes:KnownRegistrationId"];
            _knownRuntimeId = configuration["Sufficit:TestData:Telephony:Routes:KnownRuntimeId"];
            _expectedNode = configuration["Sufficit:TestData:Telephony:Routes:ExpectedNode"];

            if (Guid.TryParse(interconnectionRaw, out var interconnectionId))
                _knownInterconnectionId = interconnectionId;

            if (Guid.TryParse(registrationRaw, out var registrationId))
                _knownRegistrationId = registrationId;

            if (!string.IsNullOrWhiteSpace(adminToken))
            {
                _adminClient = new APIClientService(
                    Options.Create(new EndPointsAPIOptions
                    {
                        BaseAddress = endpointsUrl,
                        TimeOut = timeout
                    }),
                    new StaticTokenProvider(adminToken),
                    NullLogger<APIClientService>.Instance);
            }

            _output.WriteLine($"[SETUP] Base URL: {endpointsUrl}");
            _output.WriteLine($"[SETUP] Admin token configured: {!string.IsNullOrWhiteSpace(adminToken)}");
            _output.WriteLine($"[SETUP] Known interconnection configured: {_knownInterconnectionId.HasValue}");
        }

        [Fact]
        public async Task GetRegistrationStatuses_WithConfiguredInterconnection_ShouldReturnTechnicalRuntimeIds()
        {
            if (!TryGetConfiguredScenario(out var client, out var interconnectionId))
                return;

            var statuses = (await client.Telephony.Routes.GetRegistrationStatuses(interconnectionId, CancellationToken.None)).ToList();

            Assert.NotEmpty(statuses);
            Assert.All(statuses, AssertLooksCanonical);

            _output.WriteLine($"[TEST] Retrieved {statuses.Count} registration statuses for {interconnectionId}");
        }

        [Fact]
        public async Task GetRegistrationStatuses_WithConfiguredRegistrationProbe_ShouldNotReturnNotObserved()
        {
            if (!TryGetConfiguredScenario(out var client, out var interconnectionId))
                return;

            if (!_knownRegistrationId.HasValue)
            {
                _output.WriteLine("[TEST] Skipped: missing Sufficit:TestData:Telephony:Routes:KnownRegistrationId");
                return;
            }

            var statuses = (await client.Telephony.Routes.GetRegistrationStatuses(interconnectionId, CancellationToken.None)).ToList();
            var status = statuses.SingleOrDefault(item => item.RegistrationId == _knownRegistrationId.Value);

            Assert.NotNull(status);
            AssertLooksCanonical(status);
            Assert.True(status.Observed, $"Registration '{_knownRegistrationId}' should be observed. State returned: {status.State}");
            Assert.False(string.Equals(status.State, "not-observed", StringComparison.OrdinalIgnoreCase),
                $"Registration '{_knownRegistrationId}' still returned not-observed.");

            if (!string.IsNullOrWhiteSpace(_knownRuntimeId))
            {
                Assert.True(string.Equals(status.RuntimeId, _knownRuntimeId, StringComparison.OrdinalIgnoreCase),
                    $"Expected runtime id '{_knownRuntimeId}', but got '{status.RuntimeId}'.");
            }

            if (!string.IsNullOrWhiteSpace(_expectedNode))
            {
                Assert.True(string.Equals(status.Node, _expectedNode, StringComparison.OrdinalIgnoreCase),
                    $"Expected node '{_expectedNode}', but got '{status.Node}'.");
            }

            _output.WriteLine($"[TEST] Registration '{status.RegistrationId}' returned runtime '{status.RuntimeId}' with state '{status.State}' on node '{status.Node}'");
        }

        private bool TryGetConfiguredScenario(out APIClientService client, out Guid interconnectionId)
        {
            if (_adminClient is null)
            {
                _output.WriteLine("[TEST] Skipped: missing admin token (Sufficit:Authentication:Tokens:Administrator or Admin)");
                client = null!;
                interconnectionId = Guid.Empty;
                return false;
            }

            if (!_knownInterconnectionId.HasValue)
            {
                _output.WriteLine("[TEST] Skipped: missing Sufficit:TestData:Telephony:Routes:KnownInterconnectionId");
                client = null!;
                interconnectionId = Guid.Empty;
                return false;
            }

            client = _adminClient;
            interconnectionId = _knownInterconnectionId.Value;
            return true;
        }

        private static void AssertLooksCanonical(InterconnectionRegistrationOperationalStatus item)
        {
            Assert.NotEqual(Guid.Empty, item.InterconnectionId);
            Assert.NotEqual(Guid.Empty, item.RegistrationId);
            Assert.False(string.IsNullOrWhiteSpace(item.Title));
            Assert.False(string.IsNullOrWhiteSpace(item.RuntimeId));
            Assert.StartsWith("reg-", item.RuntimeId, StringComparison.OrdinalIgnoreCase);
        }
    }
}
