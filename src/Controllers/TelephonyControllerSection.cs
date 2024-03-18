using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Sufficit.Client.Controllers.Telephony;
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
            EventsPanel = new TelephonyEventsPanelControllerSection(service);
            IVR = new TelephonyIVRControllerSection(service);
            DID = new TelephonyDIDControllerSection(service);
            EndPoint = new TelephonyEndPointControllerSection(service);
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
        public TelephonyWebRTCControllerSection WebRTC { get; }


        public async Task<IEnumerable<ICallRecordBasic>> CallSearchAsync(CallSearchParameters parameters, CancellationToken cancellationToken)
        {            
            string requestEndpoint = $"{Controller}/calls";
            string query = parameters.ToQueryString();
            logger.LogTrace($"CallSearchAsync: {query}");

            string requestUri = $"{requestEndpoint}?{query}";
            var response = await httpClient.GetAsync(requestUri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
#if NET6_0_OR_GREATER
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException(content, null, response.StatusCode);
#else

                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(content);
#endif
            }
            else 
            {
                var content = await response.Content.ReadFromJsonAsync<IEnumerable<CallRecord>>();
                if (content != null) return content;
                else return new CallRecord[] { }; 
            }
        }

        #region WEB CALL BACK

        public Task<HttpResponseMessage> WebCallBack(WebCallBackRequest request, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/webcallback";
            var uri = new Uri(requestEndpoint, UriKind.Relative);
            return httpClient.PostAsJsonAsync<WebCallBackRequest>(uri, request, cancellationToken);
        }

        #endregion
    }
}
