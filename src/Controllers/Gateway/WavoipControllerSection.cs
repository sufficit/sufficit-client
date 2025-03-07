using Microsoft.Extensions.Logging;
using Sufficit.Gateway.Wavoip;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Gateway
{
    public sealed class WavoipControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = GatewayControllerSection.Controller;
        private const string Prefix = "/wavoip";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public WavoipControllerSection(IAuthenticatedControllerBase cb) : base(cb) 
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        public Task<WavoipGateway?> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by id: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/byid?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<WavoipGateway>(message, cancellationToken);
        }

        public Task Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        public Task<IEnumerable<WavoipGateway>> Search(WavoipGatewaySearchParameters parameters, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<WavoipGateway>(message, cancellationToken);
        }

        public Task Update(WavoipGateway gateway, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/update", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(gateway, null, _json);
            return Request(message, cancellationToken);
        }
    }
}
