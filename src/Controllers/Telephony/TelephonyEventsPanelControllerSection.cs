using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.Net.Http;
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
    public sealed class TelephonyEventsPanelControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/eventspanel";

        private readonly JsonSerializerOptions _json;

        public TelephonyEventsPanelControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        public Task<IEnumerable<AMIHubConnection>> GetEndpoints(CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/endpoints";
            var uri = new Uri($"{ requestEndpoint }", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<AMIHubConnection>(message, cancellationToken);
        }

        [Authorize]
        public Task<EventsPanelServiceOptions?> GetServiceOptions(CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/serviceoptions";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<EventsPanelServiceOptions>(message, cancellationToken);
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

        public Task PostUserOptions(EventsPanelUserOptions value, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/useroptions";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(value, null, _json);
            return Request(message, cancellationToken);
        }

        protected override string[]? AnonymousPaths { get; } = {
            $"{Controller}{Prefix}/endpoints",
        };
    }
}
