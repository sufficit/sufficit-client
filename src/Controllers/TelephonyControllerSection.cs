using Microsoft.Extensions.Logging;
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
        }

        public async Task<Guid> WebRTCKey()
        {
            return await _httpClient.GetFromJsonAsync<Guid>($"{Controller}/webrtckey");           
        }
    }
}
