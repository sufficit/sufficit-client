using Microsoft.Extensions.Logging;
using Sufficit.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class IdentityControllerSection
    {
        public const string Controller = "/identity";
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public IdentityControllerSection(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<IDirective>> GetDirectives(string filter, int results = 0, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/directives";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if(!string.IsNullOrWhiteSpace(filter))
                query["filter"] = filter;

            if(results > 0)
                query["results"] = results.ToString();

            var uri = new Uri($"{ requestEndpoint }?{ query }", UriKind.Relative);
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<DirectiveBase>>(uri, cancellationToken);
            if (response != null) return response;
            else return new IDirective[] { };
        }
    }
}
