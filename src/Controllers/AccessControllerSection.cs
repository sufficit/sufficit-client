using Sufficit.Identity;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class AccessControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/access";

        public AccessControllerSection (IAuthenticatedControllerBase cb) : base(cb) { }   

        public Task<IEnumerable<UserPolicyBase>> GetUserPolicies(Guid id, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/userdirectives";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<UserPolicyBase>(message, cancellationToken);
        }
    }
}
