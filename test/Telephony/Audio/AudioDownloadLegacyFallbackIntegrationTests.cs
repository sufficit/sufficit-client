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

namespace Sufficit.Client.IntegrationTests.Telephony.Audio
{
    [Trait("Category", "Integration")]
    public class AudioDownloadLegacyFallbackIntegrationTests
    {
        private readonly ITestOutputHelper _output;
        private readonly APIClientService _apiClient;
        private readonly IConfiguration _configuration;

        public AudioDownloadLegacyFallbackIntegrationTests(ITestOutputHelper output)
        {
            _output = output;

            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var baseUrl = _configuration["Sufficit:EndPoints:BaseAddress"]
                ?? throw new InvalidOperationException("EndPoints BaseAddress not configured in appsettings.json");

            var token = _configuration["Sufficit:Authentication:Tokens:Manager"]
                ?? throw new InvalidOperationException("Manager token not configured in appsettings.json");

            var timeout = uint.TryParse(_configuration["Sufficit:EndPoints:TimeOut"], out var t) ? t : 30u;

            var options = new EndPointsAPIOptions
            {
                BaseAddress = baseUrl,
                TimeOut = timeout
            };

            _apiClient = new APIClientService(
                Options.Create(options),
                new StaticTokenProvider(token),
                NullLogger<APIClientService>.Instance);
        }

        [Fact]
        public async Task StorageObjectDownload_WithKnownLegacyAudioObjectId_ShouldReturnBytes()
        {
            var enabled = string.Equals(
                _configuration["Sufficit:TestData:Audio:RunLegacyDownloadTest"],
                "true",
                StringComparison.OrdinalIgnoreCase);

            if (!enabled)
            {
                _output.WriteLine("Skipping legacy download test. Set Sufficit:TestData:Audio:RunLegacyDownloadTest=true to enable.");
                return;
            }

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(60)).Token;

            var idText = _configuration["Sufficit:TestData:Audio:KnownLegacyAudioObjectId"]
                ?? "070b4515-ea31-4da8-bd9c-f9f2eeb321cc";

            var objectId = Guid.Parse(idText);

            _output.WriteLine($"Downloading legacy audio object: {objectId}");

            var bytes = await _apiClient.Storage.Download(objectId, nocache: "1", cancellationToken);

            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);
            _output.WriteLine($"Downloaded {bytes!.Length} bytes");
        }
    }
}
