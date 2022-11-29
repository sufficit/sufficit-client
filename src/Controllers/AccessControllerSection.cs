using Microsoft.Extensions.Logging;
using Sufficit.Identity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class AccessControllerSection
    {
        public const string Controller = "/access";
        private readonly HttpClient _httpClient;

        public AccessControllerSection(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<UserPolicyBase>> GetUserPolicies(Guid id, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/userdirectives";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{ requestEndpoint }?{ query }", UriKind.Relative);
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<UserPolicyBase>>(uri, cancellationToken);
            if (result != null) { return result; }
            return Array.Empty<UserPolicyBase>();
        }
    }
}
