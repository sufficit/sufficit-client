using Microsoft.Extensions.Logging;
using Sufficit.Telephony;
using Sufficit.Telephony.Asterisk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyWebRTCControllerSection : ControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/webrtc";

        public TelephonyWebRTCControllerSection(APIClientService service) : base(service) { }    

        public Task<Guid?> GetKey(CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}{Prefix}/key";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestStruct<Guid>(message, cancellationToken);
        }

        public Task<IEnumerable<string>> GetAvailable(CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}{Prefix}/available";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<string>(message, cancellationToken);
        }
    }
}
