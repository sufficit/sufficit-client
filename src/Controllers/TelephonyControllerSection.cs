using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Sufficit.Client.Controllers.Telephony;
using Sufficit.Contacts;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class TelephonyControllerSection : ControllerSection
    {
        public const string Controller = "/telephony";

        public TelephonyControllerSection(APIClientService service) : base(service)
        {
            Audio = new TelephonyAudioControllerSection(service);
            Balance = new TelephonyBalanceControllerSection(service);
            Destination = new TelephonyDestinationControllerSection(service);
            DID = new TelephonyDIDControllerSection(service);
            EndPoint = new TelephonyEndPointControllerSection(service);
            EventsPanel = new TelephonyEventsPanelControllerSection(service);
            IVR = new TelephonyIVRControllerSection(service); 
            MusicOnHold = new TelephonyMusicOnHoldControllerSection(service);
            WebRTC = new TelephonyWebRTCControllerSection(service);
        }
    
        public async Task<Guid> WebRTCKey()
        {
            return await httpClient.GetFromJsonAsync<Guid>($"{Controller}/webrtckey");           
        }

        public TelephonyAudioControllerSection Audio { get; }
        public TelephonyBalanceControllerSection Balance { get; }
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
            logger.LogTrace("CallSearchAsync: {query}", query);

            string uri = $"{requestEndpoint}?{query}";
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestMany<CallRecord>(message, cancellationToken);
        }

        #region WEB CALL BACK

        public Task<HttpResponseMessage> WebCallBack(ExternalCallRequest request, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/webcallback";
            var uri = new Uri(requestEndpoint, UriKind.Relative);
            return httpClient.PostAsJsonAsync<ExternalCallRequest>(uri, request, cancellationToken);
        }

        #endregion

        public async Task<IEnumerable<IIdTitlePair>> Carriers (CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/carriers";
            string uri = $"{requestEndpoint}";
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestMany<Contact>(message, cancellationToken);
        }
    }
}
