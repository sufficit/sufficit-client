using Microsoft.Extensions.Logging;
using Sufficit.Client.Controllers.Telephony;
using Sufficit.Contacts;
using Sufficit.Net.Http;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class TelephonyControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/telephony";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public TelephonyControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            Asterisk = new TelephonyAsteriskControllerSection(cb);
            Audio = new TelephonyAudioControllerSection(cb);
            Balance = new TelephonyBalanceControllerSection(cb);
            CallGroup = new TelephonyCallGroupControllerSection(cb);
            Destination = new TelephonyDestinationControllerSection(cb);
            DID = new TelephonyDIDControllerSection(cb);
            EndPoint = new TelephonyEndPointControllerSection(cb);
            EventsPanel = new TelephonyEventsPanelControllerSection(cb);
            IVR = new TelephonyIVRControllerSection(cb); 
            MusicOnHold = new TelephonyMusicOnHoldControllerSection(cb);
            WebRTC = new TelephonyWebRTCControllerSection(cb);

            _logger = cb.Logger;
            _json = cb.Json;
        }
    
        public Task<Guid?> WebRTCKey()
        {
            string requestEndpoint = $"{Controller}/webrtckey";
            var uri = new Uri(requestEndpoint, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestStruct<Guid>(message, CancellationToken.None); 
        }

        public TelephonyAsteriskControllerSection Asterisk { get; }
        public TelephonyAudioControllerSection Audio { get; }
        public TelephonyBalanceControllerSection Balance { get; }
        public TelephonyCallGroupControllerSection CallGroup { get; }
        public TelephonyDestinationControllerSection Destination { get; }
        public TelephonyDIDControllerSection DID { get; }
        public TelephonyEndPointControllerSection EndPoint { get; }
        public TelephonyEventsPanelControllerSection EventsPanel { get; }
        public TelephonyIVRControllerSection IVR { get; }
        public TelephonyMusicOnHoldControllerSection MusicOnHold { get; }
        public TelephonyWebRTCControllerSection WebRTC { get; }


        public async Task<IEnumerable<ICallRecordBasic>> CallSearchAsync(CallSearchParameters parameters, CancellationToken cancellationToken)
        {            
            string requestEndpoint = $"{Controller}/calls";
            string query = parameters.ToQueryString();
            _logger.LogTrace("CallSearchAsync: {query}", query);

            string uri = $"{requestEndpoint}?{query}";
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestMany<CallRecord>(message, cancellationToken);
        }

        #region WEB CALL BACK

        public Task<HttpResponseMessage> WebCallBack(ExternalCallRequest request, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/webcallback";
            var uri = new Uri(requestEndpoint, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return SendAsync(message, cancellationToken);
        }

        #endregion

        public async Task<IEnumerable<IIdTitlePair>> Carriers (CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/carriers";
            string uri = $"{requestEndpoint}";
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestMany<Contact>(message, cancellationToken);
        }

        protected override string[]? AnonymousPaths { get; } = {
            $"{Controller}/webcallback",
        };
    }
}
