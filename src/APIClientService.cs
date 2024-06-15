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
using System.Threading;
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

        /// <summary>
        ///     Last timestamp for health checked
        /// </summary>
        public DateTime HealthChecked { get; internal set; }

        /// <summary>
        ///     Sets a value for health status, used internal. <br />
        ///     Or you can set a custom value for testing purposes
        /// </summary>
        public void Healthy(bool value = true)
        {
            // updating timestamp
            HealthChecked = DateTime.UtcNow;

            if (Available != value)
            {
                Available = value;
                OnChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        /// <summary>
        ///     Used on component initialization for ensure ready status
        /// </summary>
        public async Task GetStatus()
        {
            await _semaphore.WaitAsync();
            if (HealthChecked == DateTime.MinValue || DateTime.UtcNow.Subtract(HealthChecked).TotalMinutes > 30)
                _ = await Health(default);

            _semaphore.Release();          
        }

        public bool Available { get; private set; }

        public async Task<HealthResponse> Health(CancellationToken cancellationToken)
        {
            bool status = false;
            HealthResponse? response;
            try
            {
                response = await httpClient.GetFromJsonAsync<HealthResponse>("/health", cancellationToken);
                if (response != null)
                    status = response.Status == "Healthy";
            }
            catch (Exception ex)
            {
                response = new HealthResponse() { Status = $"UnHealthy: {ex.Message}" };
            }

            Healthy(status);
            return response ?? new HealthResponse() { Status = "UnHealthy: null response" };
        }

        #endregion

        // Used for compare changes
        private string BaseUrl { get; set; }

        /// <summary>
        ///     Status changed
        /// </summary>
        public event EventHandler? OnChanged;

        protected void OnOptionsChanged(EndPointsAPIOptions value, string? instance)
        {
            if (BaseUrl != value.BaseUrl)
            {
                BaseUrl = value.BaseUrl;
                HealthChecked = DateTime.MinValue;
            }
        }

        /// <summary>
        ///     Default request from memory for testing purposes
        /// </summary>
        public async Task<WeatherForecast[]?> WeatherForeacast()
        {
            return await httpClient.GetFromJsonAsync<WeatherForecast[]>("/WeatherForecast");
        }

        public APIClientService(IOptionsMonitor<EndPointsAPIOptions> ioptions, IHttpClientFactory clientFactory, ILogger<APIClientService> logger)
            : base(ioptions, clientFactory, logger, Json.Options)
        {          
            // Definindo controllers sections
            Access = new AccessControllerSection(this);
            Telephony = new TelephonyControllerSection(this);
            Identity = new IdentityControllerSection(this);
            Contacts = new ContactsControllerSection(this);
            Resources = new ResourcesControllerSection(this);
            Sales = new SalesControllerSection(this);
            Logging = new LoggingControllerSection(this);
            Notification = new NotificationControllerSection(this);
            Gateway = new GatewayControllerSection(this);
            Provisioning = new ProvisioningControllerSection(this);

            BaseUrl = ioptions.CurrentValue.BaseUrl;
            ioptions.OnChange(OnOptionsChanged);

            logger.LogTrace("Sufficit API Client Service instantiated with base address: {url}", options.BaseUrl);
        }

        public AccessControllerSection Access { get; }
        public ContactsControllerSection Contacts { get; }
        public GatewayControllerSection Gateway { get; }
        public IdentityControllerSection Identity { get; }
        public LoggingControllerSection Logging { get; }
        public NotificationControllerSection Notification { get; }
        public ProvisioningControllerSection Provisioning { get; }
        public ResourcesControllerSection Resources { get; }
        public SalesControllerSection Sales { get; }
        public TelephonyControllerSection Telephony { get; }
    }
}
