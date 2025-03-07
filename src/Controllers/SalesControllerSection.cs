using Microsoft.AspNetCore.Authorization;
using Sufficit.Net.Http;
using Sufficit.Sales;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class SalesControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/sales";

        public SalesControllerSection(IAuthenticatedControllerBase cb) : base(cb) { }

        [Authorize]
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


        [Authorize(Roles = Sufficit.Sales.SalesManagerRole.NormalizedName)]
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
