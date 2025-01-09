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

        public async ValueTask<string?> GetTokenAsync()
        {
            if (_accessor.HttpContext != null)
                return await _accessor.HttpContext.GetTokenAsync(ClaimTypes.AccessToken);

            return null;
        }
    }
}
