using Microsoft.Extensions.Logging;
using Sufficit.Gateway.WhatsApp;
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
    public sealed class WhatsAppQuepasaControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = GatewayControllerSection.Controller;
        private const string Prefix = "/WhatsApp/Quepasa";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public WhatsAppQuepasaControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        /// <summary>
        /// Starts (or resumes) the Quepasa pairing session for this context — returns a QR code
        /// image or a pairing code, depending on <see cref="WhatsAppQuepasaStartRequest.Mode"/>.
        /// </summary>
        public Task<WhatsAppQuepasaStartResponse?> Start(WhatsAppQuepasaStartRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("starting whatsapp quepasa session: contextId={contextId} mode={mode}", request.ContextId, request.Mode);

            var uri = new Uri($"{Controller}{Prefix}/start", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<WhatsAppQuepasaStartResponse>(message, cancellationToken);
        }

        /// <summary>
        /// Polls whether THIS SPECIFIC connection attempt (the <paramref name="token"/> from
        /// <see cref="Start"/>) has actually scanned/paired yet — a context can have several
        /// independent Quepasa sessions, so this is never contextId alone.
        /// </summary>
        public Task<WhatsAppQuepasaStatusResponse?> Status(Guid contextId, string token, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();
            query["token"] = token;

            var uri = new Uri($"{Controller}{Prefix}/status?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<WhatsAppQuepasaStatusResponse>(message, cancellationToken);
        }

        /// <summary>
        /// Live connection state for every persisted Quepasa route in a context (routes table's
        /// status column) — looked up by phone number, not the wizard's session token.
        /// </summary>
        public Task<IEnumerable<WhatsAppQuepasaRouteState>> States(Guid contextId, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/states?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<WhatsAppQuepasaRouteState>(message, cancellationToken);
        }

        /// <summary>
        /// Snapshot of the WhatsApp calls in progress right now. Pass <see cref="Guid.Empty"/> for
        /// every context (manager only).
        ///
        /// Required by any live calls view: Quepasa publishes realtime call events only on state
        /// transitions, so a call already connected when the screen opens is invisible to an
        /// event-only client until it ends. Seed from here, then apply realtime updates.
        /// </summary>
        public Task<IEnumerable<WhatsAppQuepasaActiveCall>> Calls(Guid contextId, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (contextId != Guid.Empty)
                query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/calls?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<WhatsAppQuepasaActiveCall>(message, cancellationToken);
        }

        /// <summary>
        /// Stops and reconnects one route's Quepasa session on demand (same recovery cycle
        /// Quepasa runs on itself for websocket errors) — for when a session looks stuck.
        /// </summary>
        public Task Restart(string sessionId, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["sessionid"] = sessionId;

            var uri = new Uri($"{Controller}{Prefix}/restart?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            return Request(message, cancellationToken);
        }

        /// <summary>Deliberately parks a session — keeps the pairing, no QR/pair-code needed to bring it back with <see cref="Start"/>.</summary>
        public Task Stop(string sessionId, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["sessionid"] = sessionId;

            var uri = new Uri($"{Controller}{Prefix}/stop?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            return Request(message, cancellationToken);
        }

        /// <summary>Resumes a session previously parked with <see cref="Stop"/>.</summary>
        public Task Start(string sessionId, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["sessionid"] = sessionId;

            var uri = new Uri($"{Controller}{Prefix}/start?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            return Request(message, cancellationToken);
        }

        /// <summary>
        /// Regenerates a QR code or pairing code for an EXISTING session (same SessionId) that
        /// disconnected — recovers a route without deleting/re-pairing it from scratch. Pairing
        /// mode reuses the route's already-saved phone number server-side, nothing to type.
        /// </summary>
        public Task<WhatsAppQuepasaStartResponse?> Rescan(string sessionId, string mode = "qrcode", CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["sessionid"] = sessionId;

            var uri = new Uri($"{Controller}{Prefix}/rescan?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(new WhatsAppQuepasaRescanRequest { Mode = mode }, null, _json);
            return Request<WhatsAppQuepasaStartResponse>(message, cancellationToken);
        }

        /// <summary>
        /// Retroactively sets the session's call policy to enabled (quepasa's VoIP "master gate")
        /// — needed for sessions paired before this was set automatically on every route save.
        /// </summary>
        public Task EnableCalls(string sessionId, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["sessionid"] = sessionId;

            var uri = new Uri($"{Controller}{Prefix}/enable-calls?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            return Request(message, cancellationToken);
        }

        /// <summary>
        /// On-demand "why isn't this connected" diagnostic for one route — a separate call from
        /// <see cref="States"/> so it's only fetched when someone actually asks.
        /// </summary>
        public Task<WhatsAppQuepasaDiagnostic?> Diagnose(string sessionId, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["sessionid"] = sessionId;

            var uri = new Uri($"{Controller}{Prefix}/diagnose?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<WhatsAppQuepasaDiagnostic>(message, cancellationToken);
        }
    }
}
