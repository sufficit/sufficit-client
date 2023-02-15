using Microsoft.Extensions.Logging;
using Sufficit.Client.Controllers.Telephony;
using Sufficit.Client.Extensions;
using Sufficit.Contacts;
using Sufficit.Logging;
using Sufficit.Sales;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class LoggingControllerSection
    {
        public const string Controller = "/logging";
        private readonly HttpClient _httpClient;

        public LoggingControllerSection(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<JsonLog>> GetEventsWithContent(LogSearchParameters parameters, CancellationToken cancellationToken)
        {          
            var uri = new Uri($"{Controller}/events", UriKind.Relative);

            var response = await _httpClient.PostAsJsonAsync(uri, parameters, cancellationToken);
            await response.EnsureSuccess();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Array.Empty<JsonLog>();

            return await response.Content.ReadFromJsonAsync<IEnumerable<JsonLog>>(Json.Options, cancellationToken)
                ?? Array.Empty<JsonLog>();
        }
    }
}
