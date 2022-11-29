using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.Telephony;
using Sufficit.Telephony.Asterisk.Manager;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyEventsPanelControllerSection
    {
        private static string Prefix => "/eventspanel";
        private static string Controller => TelephonyControllerSection.Controller;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public TelephonyEventsPanelControllerSection(HttpClient httpClient, ILogger logger, JsonSerializerOptions options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options;
        }

        public async Task<IEnumerable<AMIHubConnection>> GetEndpoints(CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/endpoints";
            var uri = new Uri($"{ requestEndpoint }", UriKind.Relative);
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<AMIHubConnection>>(uri, cancellationToken);
            if (result != null) { return result; }
            return Array.Empty<AMIHubConnection>();
        }

        [Authorize]
        public async Task<EventsPanelServiceOptions?> GetServiceOptions(CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/serviceoptions";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var res = await _httpClient.GetAsync(uri, cancellationToken);
            res.EnsureSuccessStatusCode();

            if (res.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await res.Content.ReadFromJsonAsync<EventsPanelServiceOptions?>(_options, cancellationToken);      
        }

        public async Task<IEnumerable<EventsPanelCardInfo>?> GetUserCards(CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/usercards";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var res = await _httpClient.GetAsync(uri, cancellationToken);
            res.EnsureSuccessStatusCode();

            if (res.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await res.Content.ReadFromJsonAsync<IEnumerable<EventsPanelCardInfo>>(_options, cancellationToken);
        }

        public async Task<EventsPanelUserOptions?> GetUserOptions(CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/useroptions";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            HttpResponseMessage res = await _httpClient.GetAsync(uri, cancellationToken);
            res.EnsureSuccessStatusCode();

            if(res.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await res.Content.ReadFromJsonAsync<EventsPanelUserOptions?>(_options, cancellationToken);                  
        }

        public async Task PostUserOptions(EventsPanelUserOptions value, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/useroptions";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            await _httpClient.PostAsJsonAsync<EventsPanelUserOptions>(uri, value, _options, cancellationToken);
        }
    }
}
