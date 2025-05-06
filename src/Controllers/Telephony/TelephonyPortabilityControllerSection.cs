using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Sufficit.EndPoints;
using Sufficit.Identity;
using Sufficit.Json;
using Sufficit.Net.Http;
using Sufficit.Telephony;
using Sufficit.Telephony.Portability;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyPortabilityControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/portability";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public TelephonyPortabilityControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        public Task<PortabilityProcess?> ById (Guid id, CancellationToken cancellationToken)
        {
            _logger.LogTrace("by id: {id}", id);
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            var uri = new Uri($"{Controller}{Prefix}/byid?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<PortabilityProcess>(message, cancellationToken);
        }

        [Authorize]
        public IAsyncEnumerable<PortabilityProcess> SearchAsAsyncEnumerable (PortabilitySearchParameters parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by search parameters: {parameters}", parameters);
            var uri = new Uri($"{Controller}{Prefix}/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestManyAsAsyncEnumerable<PortabilityProcess>(message, cancellationToken);
        }

        [Authorize]
        public Task<IEnumerable<PortabilityProcess>> Search(PortabilitySearchParameters parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by search parameters: {parameters}", parameters);
            var uri = new Uri($"{Controller}{Prefix}/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<PortabilityProcess>(message, cancellationToken);
        }

        [Authorize(Roles = $"{AdministratorRole.NormalizedName},{ManagerRole.NormalizedName},{TelephonyAdminRole.NormalizedName}")]
        public Task AddOrUpdate (PortabilityProcess item, CancellationToken cancellationToken)
        {
            _logger.LogTrace("add or update item: {item}", item.ToJsonOrDefault());
            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            return Request<EndPointResponse>(message, cancellationToken);
        }

        [Authorize(Roles = $"{AdministratorRole.NormalizedName}")]
        public Task Remove(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogTrace("remove by id: {id}", id);
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        protected override string[]? AnonymousPaths { get; } = { $"{Controller}{Prefix}/byid" };
    }
}
