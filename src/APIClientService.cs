using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.APIClient.Controllers;
using Sufficit.EndPoints.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.APIClient
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

            // Definindo controllers sections
            Telephony = new TelephonyControllerSection(_httpClient, _logger);

            _logger.LogTrace("Sufficit API Client Service instantiated");
        }

        public async Task<WeatherForecast[]?> WeatherForeacast()
        {
            return await _httpClient.GetFromJsonAsync<WeatherForecast[]>("/WeatherForecast");            
        }

        public TelephonyControllerSection Telephony { get; }        
    }
}
