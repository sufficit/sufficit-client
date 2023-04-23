using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.Client.Controllers;
using Sufficit.EndPoints.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Client
{
    /// <summary>
    /// Singleton implementation of default API Client <br />
    /// With Transient Http Client
    /// </summary>
    public class APIClientService : ControllerSection
    {                       
        public APIClientService(IOptionsMonitor<EndPointsAPIOptions> ioptions, IHttpClientFactory clientFactory, ILogger<APIClientService> logger)
            : base(ioptions, clientFactory, logger, Json.Options)
        {          
            // Definindo controllers sections
            Access = new AccessControllerSection(this);
            Telephony = new TelephonyControllerSection(this);
            Identity = new IdentityControllerSection(this);
            Contact = new ContactControllerSection(this);
            Sales = new SalesControllerSection(this);
            Logging = new LoggingControllerSection(this);
            Gateway = new GatewayControllerSection(this);
            Provisioning = new ProvisioningControllerSection(this);

            logger.LogTrace($"Sufficit API Client Service instantiated with base address: {options.BaseUrl}");
        }

        public async Task<HealthResponse?> Health()
        {
            return await httpClient.GetFromJsonAsync<HealthResponse>("/health");
        }

        public async Task<WeatherForecast[]?> WeatherForeacast()
        {
            return await httpClient.GetFromJsonAsync<WeatherForecast[]>("/WeatherForecast");            
        }

        public AccessControllerSection Access { get; }

        public TelephonyControllerSection Telephony { get; }

        public IdentityControllerSection Identity { get; }

        public ContactControllerSection Contact { get; }

        public SalesControllerSection Sales { get; }

        public LoggingControllerSection Logging { get; }

        public GatewayControllerSection Gateway { get; }

        public ProvisioningControllerSection Provisioning { get; }
    }
}
