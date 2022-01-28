using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.APIClient.Controllers.Telephony
{
    public sealed class TelephonyBalanceControllerSection
    {
        private string Controller => TelephonyControllerSection.Controller;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public TelephonyBalanceControllerSection(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task Notify(Guid idcontext, bool force, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["idcontext"] = idcontext.ToString();
            query["force"] = force.ToString();

            var uri = new Uri($"{Controller}/balance?{query}", UriKind.Relative);
            var request = new HttpRequestMessage(HttpMethod.Head, uri);            
            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
