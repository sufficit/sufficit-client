using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sufficit.Client;
using Sufficit.EndPoints.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sufficit.Client.IntegrationTests.Telephony.Audio
{
    /// <summary>
    /// Integration tests for Audio FileInfo endpoint
    /// Tests the endpoint: POST /Telephony/Audio/FileInfo
    /// 
    /// Configuration in appsettings.json:
    /// - Sufficit:EndPoints:BaseAddress - API base URL  
    /// - Sufficit:Authentication:Tokens:Manager - Bearer token for authentication
    /// </summary>
    [Trait("Category", "Integration")]
    public class AudioFileInfoIntegrationTests
    {
        private readonly ITestOutputHelper _output;
        private readonly APIClientService _apiClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        
        public AudioFileInfoIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
            
            // Load configuration from appsettings.json
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            _baseUrl = _configuration["Sufficit:EndPoints:BaseAddress"] 
                ?? throw new InvalidOperationException("EndPoints BaseAddress not configured in appsettings.json");
            
            // Use Manager token for integration tests (has full access)
            var token = _configuration["Sufficit:Authentication:Tokens:Manager"] 
                ?? throw new InvalidOperationException("Manager token not configured in appsettings.json");

            var timeout = uint.TryParse(_configuration["Sufficit:EndPoints:TimeOut"], out var t) ? t : 30u;

            // Configure EndPoints options (same pattern as ContactSearchIntegrationTests)
            var options = new EndPointsAPIOptions
            {
                BaseAddress = _baseUrl,
                TimeOut = timeout
            };

            // Create API client using the established pattern
            _apiClient = new APIClientService(
                Options.Create(options),
                new StaticTokenProvider(token),
                NullLogger<APIClientService>.Instance);
        }

        [Fact]
        public async Task FileInfo_WithKnownAudioId_ReturnsFileInfo()
        {
            // Arrange  
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token;
            
            // Use test audio ID from configuration
            var audioId = Guid.Parse(_configuration["Sufficit:TestData:Audio:KnownAudioFileId"] 
                ?? "cbecf250-8f73-477c-9faa-76b1db992049");

            _output.WriteLine($"Testing FileInfo endpoint with audio ID: {audioId}");
            _output.WriteLine($"Base URL: {_baseUrl}");
            
            // Act - Use Telephony.Audio section from APIClientService
            var result = await _apiClient.Telephony.Audio.FileInfo(audioId, cancellationToken);
            
            // Assert
            Assert.NotNull(result);
            _output.WriteLine($"FileInfo result: FormatName={result.FormatName}, FileSize={result.FileSize}");
            
            // Validate response structure (should contain audio metadata)
            Assert.NotNull(result.FormatName);
            Assert.True(result.FileSize > 0);
        }

        [Fact]
        public async Task FileInfo_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token;
            
            // Use a non-existent ID 
            var invalidId = Guid.NewGuid();

            _output.WriteLine($"Testing FileInfo endpoint with invalid ID: {invalidId}");
            _output.WriteLine($"Base URL: {_baseUrl}");
            
            // Act - Use Telephony.Audio section from APIClientService 
            var result = await _apiClient.Telephony.Audio.FileInfo(invalidId, cancellationToken);
            
            // Assert
            Assert.Null(result);
            _output.WriteLine("FileInfo correctly returned null for invalid ID");
        }
    }
}