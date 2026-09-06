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
    public sealed class WhatsAppOfficialControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = GatewayControllerSection.Controller;
        private const string Prefix = "/WhatsApp/Official";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public WhatsAppOfficialControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        public Task<WhatsAppOfficialAppConfiguration?> GetActiveApp(CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/app", UriKind.Relative));
            return Request<WhatsAppOfficialAppConfiguration>(message, cancellationToken);
        }

        public Task<IEnumerable<WhatsAppOfficialAppConfiguration>> ListApps(CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/apps", UriKind.Relative));
            return RequestMany<WhatsAppOfficialAppConfiguration>(message, cancellationToken);
        }

        public Task<WhatsAppOfficialAppConfiguration?> UpsertApp(
            WhatsAppOfficialAppUpsertRequest request,
            CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, new Uri($"{Controller}{Prefix}/apps", UriKind.Relative))
            {
                Content = JsonContent.Create(request, null, _json)
            };
            return Request<WhatsAppOfficialAppConfiguration>(message, cancellationToken);
        }

        public Task<WhatsAppOfficialAppConfiguration?> ActivateApp(
            string appKey,
            CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/apps/activate", UriKind.Relative))
            {
                Content = JsonContent.Create(new WhatsAppOfficialAppActivationRequest { AppKey = appKey }, null, _json)
            };
            return Request<WhatsAppOfficialAppConfiguration>(message, cancellationToken);
        }

        /// <summary>
        /// Enables Calling + SIP on the client's own WhatsApp Official phone number, pointing
        /// it at Sufficit's Asterisk gateway. Returns the number's E.164 digits for step 2.
        /// Provide either <see cref="WhatsAppOfficialEnableRequest.PhoneNumberId"/> or
        /// <see cref="WhatsAppOfficialEnableRequest.PhoneNumber"/> (server resolves the id by
        /// searching every WABA our Meta token has access to).
        /// </summary>
        public Task<WhatsAppOfficialEnableResponse?> Enable(WhatsAppOfficialEnableRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("enabling whatsapp official calling: phoneNumberId={phoneNumberId} phoneNumber={phoneNumber}", request.PhoneNumberId, request.PhoneNumber);

            var uri = new Uri($"{Controller}{Prefix}/enable", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<WhatsAppOfficialEnableResponse>(message, cancellationToken);
        }

        /// <summary>
        /// Completes the Meta Embedded Signup flow: exchanges the callback's <c>code</c> for an
        /// access token, resolves the client's phone number(s), and enables Calling + SIP
        /// automatically pointing at Sufficit's Asterisk gateway.
        /// </summary>
        public Task<WhatsAppEmbeddedSignupResponse?> EmbeddedSignup(WhatsAppEmbeddedSignupRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("processing whatsapp embedded signup: wabaId={wabaId} phoneNumberId={phoneNumberId}", request.WabaId, request.PhoneNumberId);

            var uri = new Uri($"{Controller}{Prefix}/embedded-signup", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<WhatsAppEmbeddedSignupResponse>(message, cancellationToken);
        }

        /// <summary>
        /// Exchanges an Embedded Signup callback code and lists the WhatsApp numbers made
        /// available to Sufficit. This is deliberately read-only: it does not register a number,
        /// subscribe the Sufficit app to messaging webhooks, or change Calling/SIP settings.
        /// </summary>
        public Task<WhatsAppEmbeddedSignupResponse?> DiscoverEmbeddedSignup(WhatsAppEmbeddedSignupRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("discovering whatsapp embedded signup numbers: wabaId={wabaId} phoneNumberId={phoneNumberId}", request.WabaId, request.PhoneNumberId);

            var uri = new Uri($"{Controller}{Prefix}/embedded-signup/discover", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<WhatsAppEmbeddedSignupResponse>(message, cancellationToken);
        }

        /// <summary>
        /// Lists every phone number a client-provided token can see — feeds autocomplete
        /// suggestions in the manual connect flow once the client pastes their token. Also
        /// reports whether the token itself is even valid, checked separately from "found zero
        /// numbers" (the two look identical otherwise, but need different fixes).
        /// </summary>
        public Task<WhatsAppOfficialListNumbersResponse?> ListNumbers(WhatsAppOfficialListNumbersRequest request, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/list-numbers", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<WhatsAppOfficialListNumbersResponse>(message, cancellationToken);
        }

        /// <summary>
        /// Reads Meta's health_status for one authorized phone number. The server resolves the
        /// opaque authorization session and never sends the underlying access token to Blazor.
        /// </summary>
        public Task<WhatsAppOfficialHealthStatusResponse?> GetHealthStatus(WhatsAppOfficialHealthStatusRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("checking whatsapp official health status: phoneNumberId={phoneNumberId}", request.PhoneNumberId);

            var uri = new Uri($"{Controller}{Prefix}/health-status", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request, null, _json)
            };
            return Request<WhatsAppOfficialHealthStatusResponse>(message, cancellationToken);
        }

        /// <summary>
        /// Reports whether this user+context still holds a valid short-lived Meta authorization
        /// (and which numbers it can see), so the page can show the "already authorized" badge
        /// and restore the number list without repeating the OAuth login.
        /// </summary>
        public Task<WhatsAppOfficialAuthorizationSessionResponse?> GetAuthorizationSession(Guid contextId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("checking whatsapp official authorization session: contextId={contextId}", contextId);

            var uri = new Uri($"{Controller}{Prefix}/authorization-session?contextid={contextId}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<WhatsAppOfficialAuthorizationSessionResponse>(message, cancellationToken);
        }

        /// <summary>
        /// Discards the stored short-lived Meta authorization for this user+context — the
        /// "use another account" action in the badge dialog. Idempotent.
        /// </summary>
        public Task DiscardAuthorizationSession(Guid contextId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("discarding whatsapp official authorization session: contextId={contextId}", contextId);

            var uri = new Uri($"{Controller}{Prefix}/authorization-session?contextid={contextId}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }
    }
}
