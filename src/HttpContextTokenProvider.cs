using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Sufficit.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sufficit.Client
{
    public class HttpContextTokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _accessor;
        public HttpContextTokenProvider(IHttpContextAccessor accessor) { _accessor = accessor; }

        public async ValueTask<string?> GetTokenAsync ()
            => await _accessor.HttpContext.GetTokenAsync("access_token");        
    }
}
