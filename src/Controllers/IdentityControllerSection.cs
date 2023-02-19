using Sufficit.Identity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class IdentityControllerSection : ControllerSection
    {
        public const string Controller = "/identity";

        public IdentityControllerSection(APIClientService service) : base(service) { }
    
        public async Task<IEnumerable<IDirective>> GetDirectives(string filter, int results = 0, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/directives";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if(!string.IsNullOrWhiteSpace(filter))
                query["filter"] = filter;

            if(results > 0)
                query["results"] = results.ToString();

            var uri = new Uri($"{ requestEndpoint }?{ query }", UriKind.Relative);
            var response = await httpClient.GetFromJsonAsync<IEnumerable<DirectiveBase>>(uri, cancellationToken);
            if (response != null) return response;
            else return new IDirective[] { };
        }
    }
}
