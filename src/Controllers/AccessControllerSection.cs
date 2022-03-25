using Microsoft.Extensions.Logging;
using Sufficit.Client.Controllers.Telephony;
using Sufficit.Client.Extensions;
using Sufficit.Contacts;
using Sufficit.Identity;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class AccessControllerSection
    {
        public const string Controller = "/access";
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public AccessControllerSection(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
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
