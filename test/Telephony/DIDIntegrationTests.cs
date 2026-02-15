using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.EndPoints.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sufficit.Client.IntegrationTests.Telephony
{
    [Trait("Category", "Integration")]
    public class DIDIntegrationTests
    {
        private readonly ITestOutputHelper _output;
        private readonly APIClientService _apiClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        private readonly string _knownDidExtension;

        public DIDIntegrationTests(ITestOutputHelper output)
        {
            _output = output;

            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            _baseUrl = _configuration["Sufficit:EndPoints:BaseAddress"]
                ?? throw new InvalidOperationException("EndPoints BaseAddress not configured in appsettings.json");

            _knownDidExtension = _configuration["Sufficit:TestData:Telephony:DID:KnownExtension"]
                ?? throw new InvalidOperationException("DID KnownExtension not configured in appsettings.json (Sufficit:TestData:Telephony:DID:KnownExtension)");

            var token = _configuration["Sufficit:Authentication:Tokens:Manager"]
                ?? throw new InvalidOperationException("Manager token not configured in appsettings.json");

            var timeout = uint.TryParse(_configuration["Sufficit:EndPoints:TimeOut"], out var t) ? t : 30u;

            var options = new EndPointsAPIOptions
            {
                BaseAddress = _baseUrl,
                TimeOut = timeout
            };

            _apiClient = new APIClientService(
                Options.Create(options),
                new StaticTokenProvider(token),
                NullLogger<APIClientService>.Instance);
        }

        [Fact]
        public async Task Activity_ShouldReturnDate_WhenExists()
        {
            var extension = _knownDidExtension;
            var cancellationToken = CancellationToken.None;

            _output.WriteLine($"Testing DID Activity for extension: {extension}");

            try
            {
                // Default days = 0 (checks full history or default behavior)
                var result = await _apiClient.Telephony.DID.Activity(extension, 0, cancellationToken);
                _output.WriteLine($"Activity result: {result}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        [Fact]
        public async Task Activity_WithDays_ShouldReturnDate_WhenExists()
        {
            var extension = _knownDidExtension;
            var days = 30;
            var cancellationToken = CancellationToken.None;

            _output.WriteLine($"Testing DID Activity for extension: {extension} with days: {days}");

            try
            {
                var result = await _apiClient.Telephony.DID.Activity(extension, days, cancellationToken);
                _output.WriteLine($"Activity result: {result}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }
    }
}
