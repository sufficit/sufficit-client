using Microsoft.Extensions.Logging;
using Sufficit.EndPoints;
using Sufficit.Exchange;
using Sufficit.Telephony;
using Sufficit.Telephony.Asterisk;
using Sufficit.Telephony.DIDs;
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
    public sealed class TelephonyDIDControllerSection : ControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/did";

        public TelephonyDIDControllerSection(APIClientService service) : base(service) { }  

        public Task<IEnumerable<DirectInwardDialing>> ByContext(Guid contextId, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("by context: {contextid}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/bycontext?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<DirectInwardDialing>(message, cancellationToken);
        }

        public Task<DirectInwardDialing?> ById(Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("by id: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/byid?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<DirectInwardDialing>(message, cancellationToken);
        }

        public Task<DirectInwardDialing?> ByExtension(string extension, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("by extension: {extension}", extension);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["extension"] = extension.ToString();

            var uri = new Uri($"{Controller}{Prefix}/byextension?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<DirectInwardDialing>(message, cancellationToken);
        }

        public Task<EndPointFullResponse<DirectInwardDialing>> FullSearch(DIDSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("by full search parameters: {parameters}", parameters);
            var uri = new Uri($"{Controller}{Prefix}/fullsearch", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return Request<EndPointFullResponse<DirectInwardDialing>>(message, cancellationToken)!;
        }

        public Task<IEnumerable<DirectInwardDialing>> Search(DIDSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("by search parameters: {parameters}", parameters);
            var uri = new Uri($"{Controller}{Prefix}/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return RequestMany<DirectInwardDialing>(message, cancellationToken);
        }

        /// <summary>
        /// Update Owner information
        /// </summary>
        public Task Owner(OwnerUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/owner", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return Request(message, cancellationToken);
        }

        /// <summary>
        /// Update Owner information
        /// </summary>
        public Task Context(ContextUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/context", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return Request(message, cancellationToken);
        }

        /// <summary>
        /// Update Filter information
        /// </summary>
        public Task Filter(FilterUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/filter", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return Request(message, cancellationToken);
        }

        /// <summary>
        /// Update Extra information
        /// </summary>
        public Task Extra(ExtraUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/extra", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return Request(message, cancellationToken);
        }

        /// <summary>
        /// Update Destination information
        /// </summary>
        public Task Destination(Guid id, DestinationBase destination, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/destination?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(destination, null, jsonOptions);
            return Request(message, cancellationToken);
        }

        /// <summary>
        /// Update basic properties
        /// </summary>
        public Task Properties(Guid id, DirectInwardDialingProperties properties, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/properties?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(properties, null, jsonOptions);
            return Request(message, cancellationToken);
        }
    }
}
