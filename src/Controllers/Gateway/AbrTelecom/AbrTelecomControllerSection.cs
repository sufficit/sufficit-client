using Microsoft.Extensions.Logging;
using Sufficit.Net.Http;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Gateway
{
    /// <summary>
    /// Typed client for the ABR Telecom consultation endpoint
    /// (<c>GET /Gateway/AbrTelecom/consulta</c>).
    /// </summary>
    public sealed class AbrTelecomControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = GatewayControllerSection.Controller;
        private const string Prefix = "/abrtelecom";

        private readonly ILogger _logger;

        public AbrTelecomControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
        }

        /// <summary>
        /// Queries the current carrier/routing situation for a phone number.
        /// Blocks until a human operator solves the reCAPTCHA server-side.
        /// </summary>
        /// <param name="areaCode">Two-digit area code (e.g. "21").</param>
        /// <param name="number">Local number without the area code (e.g. "40627711").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task<AbrTelecomQueryResult?> Consulta(
            string areaCode,
            string number,
            CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["areaCode"] = areaCode;
            query["number"] = number;

            var uri = new Uri($"{Controller}{Prefix}/consulta?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<AbrTelecomQueryResult>(message, cancellationToken);
        }
    }
}
