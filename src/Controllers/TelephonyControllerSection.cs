using Microsoft.Extensions.Logging;
using Sufficit.APIClient.Controllers.Telephony;
using Sufficit.APIClient.Extensions;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.APIClient.Controllers
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
        }

        public async Task<Guid> WebRTCKey()
        {
            return await _httpClient.GetFromJsonAsync<Guid>($"{Controller}/webrtckey");           
        }

        public TelephonyBalanceControllerSection Balance { get; }

        public async Task<IEnumerable<ICallRecordBasic>> CallSearchAsync(CallSearchParameters parameters, CancellationToken cancellationToken = default)
        {            
            string requestEndpoint = "/telephony/calls";
            string requestParams = parameters.ToUriQuery();
            _logger.LogTrace($"CallSearchAsync: {requestParams}");

            string requestUri = $"{requestEndpoint}?{requestParams}";
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<CallRecord>>(requestUri, cancellationToken);
            if (response != null) return response;
            else return new CallRecord[] { };
        }
    }
}
