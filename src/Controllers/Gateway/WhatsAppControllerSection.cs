using Microsoft.Extensions.Logging;
using Sufficit.Gateway.WhatsApp;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Gateway
{
    public sealed class WhatsAppControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = GatewayControllerSection.Controller;
        private const string Prefix = "/WhatsApp";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public WhatsAppControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
            _json = cb.Json;
            Official = new WhatsAppOfficialControllerSection(cb);
        }

        public WhatsAppOfficialControllerSection Official { get; }

        public Task<WhatsAppGatewayRoute?> AddOrUpdate(WhatsAppGatewayRouteRequest request, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<WhatsAppGatewayRoute>(message, cancellationToken);
        }

        public Task<WhatsAppGatewayRoute?> GetBySessionId(string sessionId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by session id: {sessionId}", sessionId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["sessionid"] = sessionId;

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<WhatsAppGatewayRoute>(message, cancellationToken);
        }

        public Task<IEnumerable<WhatsAppGatewayRoute>> GetByContextId(Guid contextId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by context id: {contextId}", contextId);

            var uri = new Uri($"{Controller}{Prefix}/context/{contextId}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<WhatsAppGatewayRoute>(message, cancellationToken);
        }

        public Task Delete(string sessionId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("delete by session id: {sessionId}", sessionId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["sessionid"] = sessionId;

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }
    }
}
