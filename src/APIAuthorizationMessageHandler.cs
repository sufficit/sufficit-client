using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;
using Sufficit.EndPoints.Configuration;
using System;

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
}
