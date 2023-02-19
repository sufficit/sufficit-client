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
    public sealed class TelephonyEventsPanelControllerSection : ControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/eventspanel";

        public TelephonyEventsPanelControllerSection(APIClientService service) : base(service) { }    

        public async Task<IEnumerable<AMIHubConnection>> GetEndpoints(CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/endpoints";
            var uri = new Uri($"{ requestEndpoint }", UriKind.Relative);
            var result = await httpClient.GetFromJsonAsync<IEnumerable<AMIHubConnection>>(uri, cancellationToken);
            if (result != null) { return result; }
            return Array.Empty<AMIHubConnection>();
        }

        [Authorize]
        public async Task<EventsPanelServiceOptions?> GetServiceOptions(CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/serviceoptions";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var response = await httpClient.GetAsync(uri, cancellationToken);
            await response.EnsureSuccess();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await response.Content.ReadFromJsonAsync<EventsPanelServiceOptions?>(jsonOptions, cancellationToken);      
        }

        public Task<IEnumerable<EventsPanelCardInfo>> GetCardsByUser(CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/cardsbyuser", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<EventsPanelCardInfo>(message, cancellationToken);
        }

        public Task<IEnumerable<EventsPanelCardInfo>> GetCardsByContext(Guid contextId, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/cardsbycontext?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<EventsPanelCardInfo>(message, cancellationToken);
        }

        public Task<EventsPanelUserOptions?> GetUserOptions(CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/useroptions";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<EventsPanelUserOptions>(message, cancellationToken);               
        }

        public async Task PostUserOptions(EventsPanelUserOptions value, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/useroptions";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            await httpClient.PostAsJsonAsync<EventsPanelUserOptions>(uri, value, jsonOptions, cancellationToken);
        }
    }
}
