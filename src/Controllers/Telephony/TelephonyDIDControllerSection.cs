using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Sufficit.EndPoints;
using Sufficit.Identity;
using Sufficit.Net.Http;
using Sufficit.Telephony;
using Sufficit.Telephony.DIDs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyDIDControllerSection : AuthenticatedControllerSection, DIDControllerInterface
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/did";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public TelephonyDIDControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        public Task<IEnumerable<DirectInwardDialing>> ByContext(Guid contextId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by context: {contextid}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/bycontext?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<DirectInwardDialing>(message, cancellationToken);
        }

        public Task<DirectInwardDialing?> ById(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by id: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/byid?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<DirectInwardDialing>(message, cancellationToken);
        }

        public Task<DirectInwardDialing?> ByExtension(string extension, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by extension: {extension}", extension);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["extension"] = extension.ToString();

            var uri = new Uri($"{Controller}{Prefix}/byextension?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<DirectInwardDialing>(message, cancellationToken);
        }

        [Authorize(Roles = AdministratorRole.NormalizedName)]
        public Task Remove(Guid id, string? reason = null, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("remove by id: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            query["reason"] = reason;

            var uri = new Uri($"{Controller}{Prefix}/byid?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }


        [Authorize(Roles = AdministratorRole.NormalizedName)]
        public Task Remove(string extension, string? reason = null, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("remove by extension: {extension}", extension);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["extension"] = extension;
            query["reason"] = reason;

            var uri = new Uri($"{Controller}{Prefix}/byextension?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        public Task<EndPointFullResponse<DirectInwardDialing>> FullSearch(DIDSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by full search parameters: {parameters}", parameters);
            var uri = new Uri($"{Controller}{Prefix}/fullsearch", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return Request<EndPointFullResponse<DirectInwardDialing>>(message, cancellationToken)!;
        }

        public Task<IEnumerable<DirectInwardDialing>> Search(DIDSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by search parameters: {parameters}", parameters);
            var uri = new Uri($"{Controller}{Prefix}/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<DirectInwardDialing>(message, cancellationToken);
        }

        public Task<DirectInwardDialing> AddOrUpdate(DirectInwardDialing item, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("add or update: {item}", item);
            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            return Request<DirectInwardDialing>(message, cancellationToken)!;
        }

        /// <summary>
        /// Update Owner information
        /// </summary>
        public Task Owner(OwnerUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/owner", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return Request(message, cancellationToken);
        }

        /// <summary>
        /// Update Owner information
        /// </summary>
        public Task Context(ContextUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/context", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return Request(message, cancellationToken);
        }

        /// <summary>
        /// Update Filter information
        /// </summary>
        public Task Filter(FilterUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/filter", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return Request(message, cancellationToken);
        }

        /// <summary>
        /// Update Extra information
        /// </summary>
        public Task Extra(ExtraUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/extra", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
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
            message.Content = JsonContent.Create(destination, null, _json);
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
            message.Content = JsonContent.Create(properties, null, _json);
            return Request(message, cancellationToken);
        }
    }
}
