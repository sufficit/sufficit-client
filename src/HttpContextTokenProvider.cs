using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Sufficit.Identity;
using System;
using System.Threading.Tasks;

namespace Sufficit.Client
{
    public class HttpContextTokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _accessor;
        public HttpContextTokenProvider(IHttpContextAccessor accessor) { _accessor = accessor; }

        public Task<string?> GetTokenAsync()
            => _accessor.HttpContext.GetTokenAsync("access_token");        
    }
}
