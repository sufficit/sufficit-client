using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.EndPoints.Configuration;
using Sufficit.Telephony.CallDispatch;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sufficit.Client.IntegrationTests.Telephony
{
    /// <summary>
    /// Safe integration tests for the Call Dispatch API surface.
    /// These tests are read-only and intentionally avoid starting dispatches or mutating presets.
    /// </summary>
    [Trait("Category", "Integration")]
    public sealed class CallDispatchIntegrationTests
    {
        private const uint DefaultLimit = 5;

        private readonly ITestOutputHelper _output;
        private readonly APIClientService _apiClient;
        private readonly bool _requirePublishedCallDispatch;

        public CallDispatchIntegrationTests(ITestOutputHelper output)
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
            _requirePublishedCallDispatch = bool.TryParse(configuration["Sufficit:Tests:RequirePublishedCallDispatch"], out var requirePublished)
                && requirePublished;

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
        public async Task SearchConfigurations_ReturnsAtMostRequestedLimit()
        {
            var parameters = new CallDispatchConfigurationSearchParameters
            {
                Limit = DefaultLimit,
            };

            _output.WriteLine($"Testing POST /telephony/calldispatch/configurations/search with limit={DefaultLimit}");

            var result = await ExecuteOrTreatAsUnavailable(
                () => _apiClient.Telephony.CallDispatch.SearchConfigurations(parameters, CancellationToken.None),
                "Call Dispatch configuration search endpoint");

            if (!result.Available)
                return;

            var items = result.Result!.ToList();

            Assert.NotNull(items);
            Assert.True(items.Count <= DefaultLimit, $"Expected at most {DefaultLimit} presets but got {items.Count}.");

            foreach (var item in items)
            {
                Assert.NotEqual(Guid.Empty, item.Id);
                Assert.NotEqual(Guid.Empty, item.ContextId);
                Assert.False(string.IsNullOrWhiteSpace(item.Asterisk));
                if (item.Title is not null)
                {
                    Assert.False(string.IsNullOrWhiteSpace(item.Title));
                }

                _output.WriteLine($"Preset: {item.Id} | Context: {item.ContextId} | Title: {item.Title ?? "<empty>"} | Asterisk: {item.Asterisk}");
            }
        }

        [Fact]
        [Trait("Database", "ReadOnly")]
        [Trait("Safety", "NoMutations")]
        public async Task SearchConfigurations_WithExactTitleFilter_ReturnsMatchingPresets_WhenAnyTitledPresetExists()
        {
            var discoveryParameters = new CallDispatchConfigurationSearchParameters
            {
                Limit = 20,
            };

            var discoveryResult = await ExecuteOrTreatAsUnavailable(
                () => _apiClient.Telephony.CallDispatch.SearchConfigurations(discoveryParameters, CancellationToken.None),
                "Call Dispatch configuration search endpoint");

            if (!discoveryResult.Available)
                return;

            var discoveredItems = discoveryResult.Result!.ToList();
            var sample = discoveredItems.FirstOrDefault(item => !string.IsNullOrWhiteSpace(item.Title));

            if (sample is null)
            {
                _output.WriteLine("No titled Call Dispatch preset was returned by the environment. Skipping exact title filter assertion.");
                return;
            }

            var parameters = new CallDispatchConfigurationSearchParameters
            {
                ContextId = sample.ContextId,
                Title = new Sufficit.TextFilter(sample.Title!, exactMatch: true),
                Limit = DefaultLimit,
            };

            _output.WriteLine($"Testing exact title filter with preset {sample.Id} and title '{sample.Title}'");

            var filteredResult = await ExecuteOrTreatAsUnavailable(
                () => _apiClient.Telephony.CallDispatch.SearchConfigurations(parameters, CancellationToken.None),
                "Call Dispatch configuration search endpoint");

            if (!filteredResult.Available)
                return;

            var filteredItems = filteredResult.Result!.ToList();

            Assert.NotEmpty(filteredItems);
            Assert.Contains(filteredItems, item => item.Id == sample.Id);

            foreach (var item in filteredItems)
            {
                Assert.Equal(sample.ContextId, item.ContextId);
                Assert.Equal(sample.Title, item.Title);
            }
        }

        [Fact]
        [Trait("Database", "ReadOnly")]
        [Trait("Safety", "NoMutations")]
        public async Task SearchExecutions_ReturnsAtMostRequestedLimit()
        {
            var parameters = new CallDispatchExecutionSearchParameters
            {
                Limit = DefaultLimit,
            };

            _output.WriteLine($"Testing POST /telephony/calldispatch/executions/search with limit={DefaultLimit}");

            var result = await ExecuteOrTreatAsUnavailable(
                () => _apiClient.Telephony.CallDispatch.SearchExecutions(parameters, CancellationToken.None),
                "Call Dispatch execution search endpoint");

            if (!result.Available)
                return;

            var items = result.Result!.ToList();

            Assert.NotNull(items);
            Assert.True(items.Count <= DefaultLimit, $"Expected at most {DefaultLimit} executions but got {items.Count}.");

            foreach (var item in items)
            {
                Assert.NotEqual(Guid.Empty, item.Id);
                Assert.NotEqual(Guid.Empty, item.ContextId);
                Assert.False(string.IsNullOrWhiteSpace(item.Destination));

                _output.WriteLine($"Execution: {item.Id} | Context: {item.ContextId} | Destination: {item.Destination} | Status: {item.Status}");
            }
        }

        [Fact]
        [Trait("Database", "ReadOnly")]
        [Trait("Safety", "NoMutations")]
        public async Task GetConfigurationById_WithUnknownId_ReturnsNull()
        {
            var unknownId = Guid.NewGuid();

            _output.WriteLine($"Testing GET /telephony/calldispatch/configuration?id={unknownId} expecting no content");

            var result = await ExecuteConfigurationReadOrTreatAsUnavailable(
                () => _apiClient.Telephony.CallDispatch.GetConfigurationById(unknownId, CancellationToken.None),
                "Call Dispatch configuration read endpoint");

            if (!result.Available)
                return;

            var item = result.Result;

            Assert.Null(item);
        }

        private async Task<(bool Available, T? Result)> ExecuteOrTreatAsUnavailable<T>(Func<Task<T>> action, string endpointDescription)
            where T : class
        {
            try
            {
                return (true, await action());
            }
            catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
            {
                if (_requirePublishedCallDispatch)
                    throw;

                _output.WriteLine($"{endpointDescription} returned 404 in the configured environment. Set Sufficit:Tests:RequirePublishedCallDispatch=true after publish to enforce this smoke test.");
                return (false, null);
            }
        }

        private async Task<(bool Available, CallDispatchConfiguration? Result)> ExecuteConfigurationReadOrTreatAsUnavailable(
            Func<Task<CallDispatchConfiguration?>> action,
            string endpointDescription)
        {
            try
            {
                return (true, await action());
            }
            catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
            {
                if (_requirePublishedCallDispatch)
                    throw;

                _output.WriteLine($"{endpointDescription} returned 404 in the configured environment. Set Sufficit:Tests:RequirePublishedCallDispatch=true after publish to enforce this smoke test.");
                return (false, null);
            }
        }
    }
}