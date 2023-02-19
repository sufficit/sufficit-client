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
    public sealed class SalesControllerSection : ControllerSection
    {
        public const string Controller = "/sales";

        public SalesControllerSection(APIClientService service) : base(service) { }

        public async Task<IEnumerable<IClient>> GetClients(string? filter, uint? results, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/clients";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrWhiteSpace(filter))
                query["filter"] = filter;

            if (results.HasValue)
                query["results"] = results.Value.ToString();

            var uri = new Uri($"{ requestEndpoint }?{ query }", UriKind.Relative);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                await response.EnsureSuccess();

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return Array.Empty<ClientInformation>();

                return await response.Content.ReadFromJsonAsync<IEnumerable<ClientInformation>>(jsonOptions, cancellationToken)
                    ?? Array.Empty<ClientInformation>();
            }
        }
    }
}
