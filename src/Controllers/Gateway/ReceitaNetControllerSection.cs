using Microsoft.Extensions.Logging;
using Sufficit.Contacts;
using Sufficit.Gateway.ReceitaNet;
using Sufficit.Logging;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Gateway
{
    public sealed class ReceitaNetControllerSection : ControllerSection
    {
        private const string Controller = GatewayControllerSection.Controller;
        private const string Prefix = "/receitanet";

        public ReceitaNetControllerSection(APIClientService service) : base(service) { }

        public Task<RNOptions?> GetOptions(Guid contextId, string title = "default", CancellationToken cancellationToken = default)
        {
            logger.LogTrace("by context id: {id}, title: {title}", contextId, title);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();
            query["title"] = title;

            var uri = new Uri($"{Controller}{Prefix}/options?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<RNOptions>(message, cancellationToken);
        }

        public Task Update(RNOptions options, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/options", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(options, null, jsonOptions);
            return Request(message, cancellationToken);
        }

        public Task<IEnumerable<RNDestination>> GetDestinations(Guid contextId, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("destinations by id: {contextid}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/destinations?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<RNDestination>(message, cancellationToken);
        }

        public Task Update(Guid contextId, IEnumerable<RNDestination> destinations, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/destinations?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(destinations, null, jsonOptions);
            return Request(message, cancellationToken);
        }

        public Task Delete(Guid contextId, string title = "default", CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();
            query["title"] = title;

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }
    }
}
