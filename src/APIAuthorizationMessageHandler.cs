using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;
using Sufficit.EndPoints.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
