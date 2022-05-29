using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sufficit.EndPoints.Configuration;
using Microsoft.AspNetCore.Authentication;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Sufficit.Client
{
    public class APIAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public APIAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigationManager, IOptions<EndPointsAPIOptions> options) : base(provider, navigationManager)
        {
            ConfigureHandler(authorizedUrls: GetAuthorizedUrls(options.Value));
        }        

        public static IEnumerable<string> GetAuthorizedUrls(EndPointsAPIOptions options)
        {
            if (options != null)
            {
                yield return $"{options.BaseUrl}";
            }
        }
    }
    

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
