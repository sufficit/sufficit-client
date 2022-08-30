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
        private readonly ILogger _logger;
        private string? accessToken;

        /// <summary>
        /// Testar se o accessor existe, !?
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public ProtectedApiBearerTokenHandler(IHttpContextAccessor httpContextAccessor, ILogger<ProtectedApiBearerTokenHandler> logger)
        {
            _accessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (ShouldAuthenticate(request)) 
            {
                if (_accessor.HttpContext != null)
                {
                    // request the access token
                    accessToken = await _accessor.HttpContext.GetTokenAsync("access_token");
                } 
                else { _logger.LogWarning("http context not available at this time, you should lead with that !"); }

                if(string.IsNullOrWhiteSpace(accessToken))
                    throw new Exception("access token not available at this time");

                // set the bearer token to the outgoing request
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);                
            }

            // Proceed calling the inner handler, that will actually send the request
            // to our protected api
            return await base.SendAsync(request, cancellationToken);
        }

        protected bool ShouldAuthenticate(HttpRequestMessage request)
        {
            switch (request.RequestUri?.AbsolutePath)
            {
                case "/contact":
                case "/identity/directives":
                case "/telephony/eventspanel/endpoints":
                case "/telephony/webcallback": return false;
                default: return true;
            }
        }
    }
}
