using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Client
{
    public class ProtectedApiBearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _accessor;
        public ProtectedApiBearerTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _accessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // request the access token
            string accessToken = await _accessor.HttpContext.GetTokenAsync("access_token");

            // set the bearer token to the outgoing request
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            // Proceed calling the inner handler, that will actually send the request
            // to our protected api
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
