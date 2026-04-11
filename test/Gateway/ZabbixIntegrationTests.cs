using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.EndPoints.Configuration;
using Sufficit.Gateway.Zabbix;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sufficit.Client.IntegrationTests.Gateway
{
    /// <summary>
    /// Safe integration tests for the Zabbix gateway API surface.
    /// These tests are read-only and intentionally avoid creating integrations or starting alerts.
    /// </summary>
    [Trait("Category", "Integration")]
    public sealed class ZabbixIntegrationTests
    {
        private const uint DefaultLimit = 5;

        private readonly ITestOutputHelper _output;
        private readonly APIClientService _apiClient;

        public ZabbixIntegrationTests(ITestOutputHelper output)
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
        [Trait("Database", "ReadOnly")]
        [Trait("Safety", "NoMutations")]
        public async Task SearchIntegrations_ReturnsAtMostRequestedLimit()
        {
            var parameters = new ZabbixGatewaySearchParameters
            {
                Limit = DefaultLimit,
            };

            _output.WriteLine($"Testing POST /gateway/zabbix/search with limit={DefaultLimit}");

            var items = (await _apiClient.Gateway.Zabbix.Search(parameters, CancellationToken.None)).ToList();

            Assert.NotNull(items);
            Assert.True(items.Count <= DefaultLimit, $"Expected at most {DefaultLimit} integrations but got {items.Count}.");

            foreach (var item in items)
            {
                Assert.NotEqual(Guid.Empty, item.Id);
                Assert.NotEqual(Guid.Empty, item.ContextId);

                if (item.Digit.HasValue)
                {
                    Assert.InRange(item.Digit.Value, ZabbixGatewayIntegration.MinimumDigit, ZabbixGatewayIntegration.MaximumDigit);
                }

                if (item.CallDispatchId.HasValue)
                {
                    Assert.NotEqual(Guid.Empty, item.CallDispatchId.Value);
                }

                _output.WriteLine($"Integration: {item.Id} | Context: {item.ContextId} | Title: {item.Title ?? "<empty>"} | CallDispatch: {item.CallDispatchId?.ToString() ?? "<empty>"}");
            }
        }

        [Fact]
        [Trait("Database", "ReadOnly")]
        [Trait("Safety", "NoMutations")]
        public async Task SearchExecutions_AndAttempts_ReturnValidCorrelationShape()
        {
            var parameters = new ZabbixAlertExecutionSearchParameters
            {
                Limit = DefaultLimit,
            };

            _output.WriteLine($"Testing POST /gateway/zabbix/executions with limit={DefaultLimit}");

            var executions = (await _apiClient.Gateway.Zabbix.SearchExecutions(parameters, CancellationToken.None)).ToList();

            Assert.NotNull(executions);
            Assert.True(executions.Count <= DefaultLimit, $"Expected at most {DefaultLimit} executions but got {executions.Count}.");

            foreach (var execution in executions)
            {
                Assert.NotEqual(Guid.Empty, execution.Id);
                Assert.NotEqual(Guid.Empty, execution.ContextId);
                Assert.NotEqual(Guid.Empty, execution.IntegrationId);

                if (execution.CallDispatchId.HasValue)
                {
                    Assert.NotEqual(Guid.Empty, execution.CallDispatchId.Value);
                }

                _output.WriteLine($"Execution: {execution.Id} | Integration: {execution.IntegrationId} | CallDispatch: {execution.CallDispatchId?.ToString() ?? "<empty>"} | Status: {execution.Status} | Trigger: {execution.Trigger}");
            }

            var sampleExecution = executions.FirstOrDefault();
            if (sampleExecution is null)
            {
                _output.WriteLine("No persisted Zabbix executions were returned by the environment. Skipping attempts correlation assertion.");
                return;
            }

            _output.WriteLine($"Testing GET /gateway/zabbix/attempts?id={sampleExecution.Id}");

            var attempts = (await _apiClient.Gateway.Zabbix.GetAttempts(sampleExecution.Id, CancellationToken.None)).ToList();

            Assert.NotNull(attempts);

            foreach (var attempt in attempts)
            {
                Assert.NotEqual(Guid.Empty, attempt.Id);
                Assert.Equal(sampleExecution.Id, attempt.AlertId);
                Assert.Equal(sampleExecution.ContextId, attempt.ContextId);
                Assert.False(string.IsNullOrWhiteSpace(attempt.PhoneNumber));

                if (attempt.DispatchId.HasValue)
                {
                    Assert.NotEqual(Guid.Empty, attempt.DispatchId.Value);
                }

                _output.WriteLine($"Attempt: {attempt.Id} | Alert: {attempt.AlertId} | Dispatch: {attempt.DispatchId?.ToString() ?? "<empty>"} | Phone: {attempt.PhoneNumber} | Status: {attempt.Status}");
            }
        }
    }
}