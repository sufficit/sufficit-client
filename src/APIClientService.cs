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
        #region STATUS MONITOR

        public bool Available { get; protected set; }

        // Used for compare changes
        private string BaseUrl { get; set; }

        public event EventHandler? OnChanged;

        protected async void OnOptionsChanged(EndPointsAPIOptions value, string? instance)
        {
            if (BaseUrl != value.BaseUrl)
            {
                BaseUrl = value.BaseUrl;
                _ = await Health();
            }
        }

        public async Task<HealthResponse> Health()
        {
            bool status = false;
            HealthResponse? response = null;
            try
            {
                response = await httpClient.GetFromJsonAsync<HealthResponse>("/health");
                if (response != null)
                    status = response.Status == "Healthy";
            }
            catch (Exception ex)
            {
                response = new HealthResponse() { Status = $"UnHealthy: {ex.Message}" };
            }

            if (Available != status)
            {
                Available = status;
                OnChanged?.Invoke(this, EventArgs.Empty);
            }

            return response ?? new HealthResponse() { Status = $"UnHealthy: null response" };
        }

        #endregion

        public APIClientService(IOptionsMonitor<EndPointsAPIOptions> ioptions, IHttpClientFactory clientFactory, ILogger<APIClientService> logger)
            : base(ioptions, clientFactory, logger, Json.Options)
        {          
            // Definindo controllers sections
            Access = new AccessControllerSection(this);
            Telephony = new TelephonyControllerSection(this);
            Identity = new IdentityControllerSection(this);
            Contacts = new ContactsControllerSection(this);
            Sales = new SalesControllerSection(this);
            Logging = new LoggingControllerSection(this);
            Gateway = new GatewayControllerSection(this);
            Provisioning = new ProvisioningControllerSection(this);

            // status monitor
            Available = true;
            BaseUrl = ioptions.CurrentValue.BaseUrl;
            ioptions.OnChange(OnOptionsChanged);

            logger.LogTrace($"Sufficit API Client Service instantiated with base address: {options.BaseUrl}");
        }


        public async Task<WeatherForecast[]?> WeatherForeacast()
        {
            return await httpClient.GetFromJsonAsync<WeatherForecast[]>("/WeatherForecast");            
        }

        public AccessControllerSection Access { get; }

        public TelephonyControllerSection Telephony { get; }

        public IdentityControllerSection Identity { get; }

        public ContactsControllerSection Contacts { get; }

        public SalesControllerSection Sales { get; }

        public LoggingControllerSection Logging { get; }

        public GatewayControllerSection Gateway { get; }

        public ProvisioningControllerSection Provisioning { get; }
    }
}
