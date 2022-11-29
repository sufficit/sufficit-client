using Microsoft.Extensions.Logging;
using Sufficit.Client.Controllers.Telephony;
using Sufficit.Client.Extensions;
using Sufficit.Contacts;
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
    public sealed class SalesControllerSection
    {
        public const string Controller = "/sales";
        private readonly HttpClient _httpClient;

        public SalesControllerSection(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<IClient>> GetClients(string? filter, uint results = 10, CancellationToken cancellationToken = default)
        {            
            string requestEndpoint = $"{Controller}/clients";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrWhiteSpace(filter))
                query["filter"] = filter;

            if (results > 0)
                query["results"] = results.ToString();

            var uri = new Uri($"{ requestEndpoint }?{ query }", UriKind.Relative);
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<ClientInformation>>(uri, cancellationToken);
            if (response != null) return response;             
            else return new ClientInformation[] { };
        }
    }
}
