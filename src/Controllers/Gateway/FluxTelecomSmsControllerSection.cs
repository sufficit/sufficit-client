using Microsoft.Extensions.Logging;
using Sufficit.Gateway.FluxTelecom.SMS;
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
    public sealed class FluxTelecomSmsControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = GatewayControllerSection.Controller;
        private const string Prefix = "/fluxtelecomsms";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public FluxTelecomSmsControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        public Task<FluxTelecomSmsGatewayStatus?> GetStatus(CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/status", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<FluxTelecomSmsGatewayStatus>(message, cancellationToken);
        }

        public async Task<int?> GetBalance(CancellationToken cancellationToken = default)
        {
            var status = await GetStatus(cancellationToken).ConfigureAwait(false);
            return status?.AvailableCredits;
        }

        public Task<List<FluxTelecomPortalUserEntry>?> ListUsers(CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/users", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<List<FluxTelecomPortalUserEntry>>(message, cancellationToken);
        }

        public Task<FluxTelecomPortalUserAuthorizedIpUpdateResult?> EnsureAuthorizedIps(FluxTelecomPortalUserAuthorizedIpUpdateRequest request, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/authorizedips", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<FluxTelecomPortalUserAuthorizedIpUpdateResult>(message, cancellationToken);
        }

        public Task<FluxTelecomSimpleMessageSendResult?> SendSimpleMessage(FluxTelecomSimpleMessageRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("manual flux telecom simple send: recipients={recipients}", request?.Recipients?.Count ?? 0);

            var uri = new Uri($"{Controller}{Prefix}/sendsimple", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<FluxTelecomSimpleMessageSendResult>(message, cancellationToken);
        }

        public Task<FluxTelecomJsonSendResponse?> SendJsonMessage(FluxTelecomJsonMessageRequest request, bool useConfiguredCallback = true, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("manual flux telecom JSON send: to={to}, configuredCallback={configuredCallback}", request?.To, useConfiguredCallback);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["useConfiguredCallback"] = useConfiguredCallback.ToString();

            var uri = new Uri($"{Controller}{Prefix}/send?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<FluxTelecomJsonSendResponse>(message, cancellationToken);
        }

        public Task<FluxTelecomJsonSendResponse?> SendJsonBatch(FluxTelecomJsonBatchRequest request, bool useConfiguredCallback = true, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["useConfiguredCallback"] = useConfiguredCallback.ToString();

            var uri = new Uri($"{Controller}{Prefix}/sendbatch?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<FluxTelecomJsonSendResponse>(message, cancellationToken);
        }

        public Task<FluxTelecomMessageStatusResponse?> QueryMessageStatuses(FluxTelecomMessageStatusLookupRequest request, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/statuses", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<FluxTelecomMessageStatusResponse>(message, cancellationToken);
        }
    }
}