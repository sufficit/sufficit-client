using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Sufficit.Gateway.PhoneVox;
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
    public sealed class PhoneVoxControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = GatewayControllerSection.Controller;
        private const string Prefix = "/phonevox";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public PhoneVoxControllerSection(IAuthenticatedControllerBase cb) : base(cb) 
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        public Task<PhoneVoxOptions?> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by id: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/options?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<PhoneVoxOptions>(message, cancellationToken);
        }

        public Task Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        public Task<IEnumerable<PhoneVoxOptions>> GetByContextId(Guid contextId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by context id: {id}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/bycontextid?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<PhoneVoxOptions>(message, cancellationToken);
        }

        public Task Update(PhoneVoxOptions options, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/options", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(options, null, _json);
            return Request(message, cancellationToken);
        }

        [Authorize]
        public Task<IEnumerable<PhoneVoxDestination>> GetDestinations(Guid contextId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("destinations by id: {contextid}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/destinations?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<PhoneVoxDestination>(message, cancellationToken);
        }

        public Task Update(Guid id, IEnumerable<PhoneVoxDestination> destinations, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/destinations?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(destinations, null, _json);
            return Request(message, cancellationToken);
        }
    }
}
