using Microsoft.Extensions.Logging;
using Sufficit.Telephony.Asterisk.Manager;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyEventsPanelControllerSection
    {
        private static string Prefix => "/eventspanel";
        private static string Controller => TelephonyControllerSection.Controller;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public TelephonyEventsPanelControllerSection(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<AMIHubConnection>> GetEndpoints(CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/endpoints";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            //query["id"] = id.ToString();

            var uri = new Uri($"{ requestEndpoint }?{ query }", UriKind.Relative);
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<AMIHubConnection>>(uri, cancellationToken);
            if (result != null) { return result; }
            return Array.Empty<AMIHubConnection>();
        }

        public async Task<EventsPanelServiceOptions?> GetOptions(CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/options";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true));
            options.PropertyNameCaseInsensitive = true;
            return await _httpClient.GetFromJsonAsync<EventsPanelServiceOptions>(uri, options, cancellationToken);           
        }
    }
}
