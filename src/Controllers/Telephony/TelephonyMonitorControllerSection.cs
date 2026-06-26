using Sufficit.Net.Http;
using Sufficit.Telephony.Monitor;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyMonitorControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/monitor";

        private readonly JsonSerializerOptions _json;

        public TelephonyMonitorControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        public Task<TelephonyMonitorActionResult?> StartAction(TelephonyMonitorActionRequest request, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/actions", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request, null, _json)
            };

            return Request<TelephonyMonitorActionResult>(message, cancellationToken);
        }

        public Task<TelephonyMonitorActionResult?> Listen(TelephonyMonitorActionRequest request, CancellationToken cancellationToken = default)
        {
            request.Mode = TelephonyMonitorActionMode.Listen;
            return StartAction(request, cancellationToken);
        }

        public Task<TelephonyMonitorActionResult?> Whisper(TelephonyMonitorActionRequest request, CancellationToken cancellationToken = default)
        {
            request.Mode = TelephonyMonitorActionMode.Whisper;
            return StartAction(request, cancellationToken);
        }
    }
}
