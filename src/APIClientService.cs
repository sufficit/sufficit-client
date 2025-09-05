using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.Client.Controllers;
using Sufficit.Client.Controllers.Finance;
using Sufficit.Client.Controllers.Identity;
using Sufficit.Client.Controllers.Notification;
using Sufficit.Client.Controllers.Reports;
using Sufficit.Client.Controllers.Storage;
using Sufficit.EndPoints.Configuration;
using Sufficit.Identity;
using Sufficit.Json;
using Sufficit.Net.Http;
using System;
using System.Net.Http;

namespace Sufficit.Client
{
    /// <summary>
    /// Scoped implementation of default API Client <br />
    /// With Transient Http Client (Authenticated)
    /// </summary>
    public class APIClientService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _client;

        /// <summary>
        ///     Status changed
        /// </summary>
        public event EventHandler? OnChanged;

        public void OnHealthChanged (object? _, bool __) 
            => OnChanged?.Invoke(this, new EventArgs());

        public APIClientService(IOptions<EndPointsAPIOptions> options, ITokenProvider tokens, ILogger<APIClientService> logger)            
        {          
            _logger = logger; 
            _client = new PreConfiguredHttpClient(options.Value);

            // setting health controller section
            Health = new HealthCheckController(_client);
            Health.OnChanged += OnHealthChanged;

            var cb = new AuthenticatedControllerBase(tokens, _client, Sufficit.Json.JsonSerializer.Options, logger) { Healthy = Health.Healthy };

            // setting controllers sub sections
            Access = new AccessControllerSection(cb);
            Audio = new AudioControllerSection(cb);
            Contacts = new ContactsControllerSection(cb);
            Exchange = new ExchangeControllerSection(cb);
            Finance = new FinanceControllerSection(cb);
            Gateway = new GatewayControllerSection(cb);
            Identity = new IdentityControllerSection(cb);
            Logging = new LoggingControllerSection(cb);
            Notification = new NotificationControllerSection(cb);
            Provisioning = new ProvisioningControllerSection(cb);
            Reports = new ReportsControllerSection(cb);
            Resources = new ResourcesControllerSection(cb);
            Sales = new SalesControllerSection(cb);
            Statistics = new StatisticsControllerSection(cb);
            Storage = new StorageControllerSection(cb);
            Tasks = new TasksControllerSection(cb);
            Telephony = new TelephonyControllerSection(cb);

            var jsonOptions = options.Value.ToJson();
            _logger.LogTrace("Sufficit API Client Service instantiated with options: {options}", jsonOptions);
        }

        public HealthCheckController Health { get; }
        public AccessControllerSection Access { get; }
        public AudioControllerSection Audio { get; }
        public ContactsControllerSection Contacts { get; }
        public ExchangeControllerSection Exchange { get; }
        public FinanceControllerSection Finance { get; }
        public GatewayControllerSection Gateway { get; }
        public IdentityControllerSection Identity { get; }
        public LoggingControllerSection Logging { get; }
        public NotificationControllerSection Notification { get; }
        public ProvisioningControllerSection Provisioning { get; }
        public ReportsControllerSection Reports { get; }
        public ResourcesControllerSection Resources { get; }
        public SalesControllerSection Sales { get; }
        public StatisticsControllerSection Statistics { get; }
        public StorageControllerSection Storage { get; }
        public TasksControllerSection Tasks { get; }
        public TelephonyControllerSection Telephony { get; }
    }
}