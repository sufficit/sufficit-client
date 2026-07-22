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
    }
}
