using Microsoft.Extensions.Logging;
using Sufficit.Gateway.Zabbix;
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
    public sealed class ZabbixControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = GatewayControllerSection.Controller;
        private const string Prefix = "/zabbix";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public ZabbixControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        public Task<ZabbixGatewayIntegration?> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by id: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/byid?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<ZabbixGatewayIntegration>(message, cancellationToken);
        }

        public Task<IEnumerable<ZabbixGatewayIntegration>> Search(ZabbixGatewaySearchParameters parameters, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<ZabbixGatewayIntegration>(message, cancellationToken);
        }

        public Task<ZabbixGatewayIntegration?> Update(ZabbixGatewayIntegration integration, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/update", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(integration, null, _json);
            return Request<ZabbixGatewayIntegration>(message, cancellationToken);
        }

        public Task Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        public Task<IEnumerable<ZabbixGatewayDestination>> GetDestinations(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("destinations by id: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/destinations?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ZabbixGatewayDestination>(message, cancellationToken);
        }

        public Task<IEnumerable<ZabbixGatewayDestination>> UpdateDestinations(Guid id, IEnumerable<ZabbixGatewayDestination> destinations, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/destinations?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(destinations, null, _json);
            return RequestMany<ZabbixGatewayDestination>(message, cancellationToken);
        }

        public Task<IEnumerable<ZabbixAlertExecution>> SearchExecutions(ZabbixAlertExecutionSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/executions", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<ZabbixAlertExecution>(message, cancellationToken);
        }

        public Task<IEnumerable<ZabbixAlertAttempt>> GetAttempts(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("attempts by alert id: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/attempts?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ZabbixAlertAttempt>(message, cancellationToken);
        }

        public Task DeleteDestination(Guid id, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/destinations?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }
    }
}