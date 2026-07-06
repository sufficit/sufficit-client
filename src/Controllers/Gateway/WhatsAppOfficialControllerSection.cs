using Microsoft.Extensions.Logging;
using Sufficit.Gateway.WhatsApp;
using Sufficit.Net.Http;
using System;
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
    }
}
