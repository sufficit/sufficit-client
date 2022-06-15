using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.Client.Controllers;
using Sufficit.EndPoints.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Client
{
    public class APIClientService
    {
        private readonly IOptions<EndPointsAPIOptions> _options;
        private readonly IHttpClientFactory _factory;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public APIClientService(IOptions<EndPointsAPIOptions> options, IHttpClientFactory clientFactory, ILogger<APIClientService> logger)
        {
            _options = options;
            _factory = clientFactory;
            _logger = logger;

            _httpClient = _factory.CreateClient(_options.Value.ClientId);
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);

            // Definindo controllers sections
            Access = new AccessControllerSection(_httpClient, _logger);
            Telephony = new TelephonyControllerSection(_httpClient, _logger);
            Identity = new IdentityControllerSection(_httpClient, _logger);
            Contact = new ContactControllerSection(_httpClient, _logger);

            _logger.LogTrace($"Sufficit API Client Service instantiated with base address: {options.Value.BaseUrl}");
        }

        public async Task<HealthResponse?> Health()
        {
            return await _httpClient.GetFromJsonAsync<HealthResponse>("/health");
        }

        public async Task<WeatherForecast[]?> WeatherForeacast()
        {
            return await _httpClient.GetFromJsonAsync<WeatherForecast[]>("/WeatherForecast");            
        }

        public AccessControllerSection Access { get; }

        public TelephonyControllerSection Telephony { get; }

        public IdentityControllerSection Identity { get; }

        public ContactControllerSection Contact { get; }
    }
}
