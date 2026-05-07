using Microsoft.Extensions.Logging;
using Sufficit.EndPoints;
using Sufficit.Net.Http;
using Sufficit.Telephony.CallForward;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyCallForwardControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/callforward";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public TelephonyCallForwardControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        /// <summary>
        /// Search call-forward rules with parameters.
        /// </summary>
        public Task<IEnumerable<CallForwardApplication>> Search(CallForwardSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("search by parameters: {?}", parameters);

            var uri = new Uri($"{Controller}{Prefix}/Search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<CallForwardApplication>(message, cancellationToken);
        }

        /// <summary>
        /// Add or update a call-forward rule.
        /// </summary>
        public async Task<CallForwardApplication?> AddOrUpdate(CallForwardApplication item, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("add or update call-forward: {id}", item.Id);

            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            var response = await Request<EndPointResponse<CallForwardApplication>>(message, cancellationToken);
            return response?.Success ?? false ? response.Data : null;
        }

        /// <summary>
        /// Delete a call-forward rule by identifier.
        /// </summary>
        public Task Delete(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("delete call-forward: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[CallForwardSearchParameters.CALLFORWARDID] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        /// <summary>
        /// Returns the available outbound CallerID options (DID numbers) for a context.
        /// </summary>
        public Task<IEnumerable<CallForwardCallerIdOption>> GetCallerIdOptions(Guid contextId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("get callerid options for context: {contextId}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/callerid?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<CallForwardCallerIdOption>(message, cancellationToken);
        }

        /// <summary>
        /// Returns a diagnostic snapshot for a call-forward rule.
        /// Checks DB persistence, destination validity and expected runtime state.
        /// </summary>
        public Task<CallForwardDiagnosticResult?> Check(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("check call-forward diagnostic: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[CallForwardSearchParameters.CALLFORWARDID] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/Check?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<CallForwardDiagnosticResult>(message, cancellationToken);
        }

        /// <summary>
        /// Probes the Asterisk dialplan via AMI to verify a call-forward rule
        /// is registered in the <c>sufficit-app-forward</c> context.
        /// </summary>
        public Task<CallForwardAsteriskProbeResult?> AsteriskProbe(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("asterisk probe call-forward: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[CallForwardSearchParameters.CALLFORWARDID] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/AsteriskProbe?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<CallForwardAsteriskProbeResult>(message, cancellationToken);
        }
    }
}
