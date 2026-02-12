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
    public sealed class TelephonyPortabilityControllerSection : AuthenticatedControllerSection, IPortabilityController
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
        public async Task<PortabilityProcess?> AddOrUpdate (PortabilityProcess item, CancellationToken cancellationToken)
        {
            _logger.LogTrace("add or update item: {item}", item.ToJsonOrDefault());
            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            var response = await Request<EndPointResponse<PortabilityProcess>>(message, cancellationToken);
            return response?.Success ?? false ? response.Data : null;
        }

        [Authorize]
        public async Task<PortabilityProcess?> Status (PortabilityProcessStatusUpdateRequest parameters, CancellationToken cancellationToken)
        {
            _logger.LogTrace("update status: {parameters}", parameters.ToJsonOrDefault());
            var uri = new Uri($"{Controller}{Prefix}/status", UriKind.Relative);
#if NET5_0_OR_GREATER
            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
#else
            var message = new HttpRequestMessage(HttpMethod.Put, uri);
#endif
            message.Content = JsonContent.Create(parameters, null, _json);
            var response = await Request<EndPointResponse<PortabilityProcess>>(message, cancellationToken);
            return response?.Success ?? false ? response.Data : null;
        }

        [Authorize(Roles = $"{AdministratorRole.NormalizedName}")]
        public async Task<int> Remove(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogTrace("remove by id: {id}", id);
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            var response = await Request<EndPointResponse>(message, cancellationToken);
            return response?.Success ?? false ? 1 : 0;
        }

        [Authorize(Roles = $"{AdministratorRole.NormalizedName},{ManagerRole.NormalizedName}")]
        public async Task<int> AddOrUpdateNote(PortabilityNote note, CancellationToken cancellationToken)
        {
            _logger.LogTrace("add or update note for process id: {processId}, text: {text}, public: {public}, timestamp: {timestamp}", 
                note.ProcessId, note.Text, note.Public, note.Timestamp);
            
            var uri = new Uri($"{Controller}{Prefix}/notes", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(note, null, _json)
            };
            
            var response = await Request<EndPointResponse>(message, cancellationToken);
            return response?.Success ?? false ? 1 : 0;
        }

        public async Task<ICollection<PortabilityNote>> GetNotes(Guid id, bool? @public, CancellationToken cancellationToken)
        {
            _logger.LogTrace("get notes for process id: {id}, public: {public}", id, @public);
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            if (@public.HasValue)
                query["public"] = @public.Value.ToString().ToLowerInvariant();
            
            var uri = new Uri($"{Controller}{Prefix}/notes?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await Request<EndPointResponse<ICollection<PortabilityNote>>>(message, cancellationToken);
            return response?.Success ?? false && response.Data != null ? response.Data : new List<PortabilityNote>();
        }

        [Authorize(Roles = $"{AdministratorRole.NormalizedName},{ManagerRole.NormalizedName}")]
        public async Task<int> RemoveNote(Guid id, DateTime timestamp, CancellationToken cancellationToken)
        {
            _logger.LogTrace("remove note from process id: {id}, timestamp: {timestamp}", id, timestamp);
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            query["timestamp"] = timestamp.ToString("o"); // ISO 8601 format
            
            var uri = new Uri($"{Controller}{Prefix}/notes?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            var response = await Request<EndPointResponse>(message, cancellationToken);
            return response?.Success ?? false ? 1 : 0;
        }

        protected override string[]? AnonymousPaths { get; } = { $"{Controller}{Prefix}/byid" };
    }
}
