using Microsoft.Extensions.Logging;
using Sufficit.Client.Controllers.Telephony;
using Sufficit.Client.Extensions;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class TelephonyControllerSection
    {
        public const string Controller = "/telephony";
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public TelephonyControllerSection(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            Balance = new TelephonyBalanceControllerSection(_httpClient, _logger);
            EventsPanel = new TelephonyEventsPanelControllerSection(_httpClient, _logger);
            IVR = new TelephonyIVRControllerSection(_httpClient, _logger);
            Audio = new TelephonyAudioControllerSection(_httpClient, _logger);
        }

        public async Task<Guid> WebRTCKey()
        {
            return await _httpClient.GetFromJsonAsync<Guid>($"{Controller}/webrtckey");           
        }

        public TelephonyBalanceControllerSection Balance { get; }

        public TelephonyEventsPanelControllerSection EventsPanel { get; }

        public TelephonyIVRControllerSection IVR { get; }

        public TelephonyAudioControllerSection Audio { get; }

        public async Task<IEnumerable<ICallRecordBasic>> CallSearchAsync(CallSearchParameters parameters, CancellationToken cancellationToken = default)
        {            
            string requestEndpoint = $"{Controller}/calls";
            string requestParams = parameters.ToUriQuery();
            _logger.LogTrace($"CallSearchAsync: {requestParams}");

            string requestUri = $"{requestEndpoint}?{requestParams}";
            var response = await _httpClient.GetAsync(requestUri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
#if NET6_0_OR_GREATER
                throw new HttpRequestException(content, null, response.StatusCode);
#else
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
            return _httpClient.PostAsJsonAsync<WebCallBackRequest>(uri, request, cancellationToken);
        }

#endregion
    }
}
