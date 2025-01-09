using Microsoft.Extensions.Logging;
using Sufficit.Client.Controllers.Telephony;
using Sufficit.Contacts;
using Sufficit.Logging;
using Sufficit.Net.Http;
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
    public sealed class SalesControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/sales";

        public SalesControllerSection(IAuthenticatedControllerBase cb) : base(cb) { }

        public Task<IEnumerable<ClientInformation>> GetClients(string? filter, uint? results, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/clients";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrWhiteSpace(filter))
                query["filter"] = filter;

            if (results.HasValue)
                query["results"] = results.Value.ToString();

            var uri = new Uri($"{ requestEndpoint }?{ query }", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ClientInformation>(message, cancellationToken);
        }

        public Task<IEnumerable<Contract>> GetContracts(ContractSearchParameters parameters, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/contract/search";
            var query = parameters.ToQueryString();
            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<Contract>(message, cancellationToken);
        }
    }
}
