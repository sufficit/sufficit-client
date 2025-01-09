using Sufficit.Identity;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class IdentityControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/identity";

        public IdentityControllerSection(IAuthenticatedControllerBase cb) : base(cb) { }

        public Task<IEnumerable<DirectiveBase>> GetDirectives(string filter, int results = 0, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/directives";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if(!string.IsNullOrWhiteSpace(filter))
                query["filter"] = filter;

            if(results > 0)
                query["results"] = results.ToString();

            var uri = new Uri($"{ requestEndpoint }?{ query }", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<DirectiveBase>(message, cancellationToken);
        }

        protected override string[]? AnonymousPaths { get; } = { $"{Controller}/directives" };
    }
}
