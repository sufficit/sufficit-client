using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public static class HttpExtensions
    {
        /// <summary>
        ///     Nearly the HttpResponseMessage.EnsureSuccessStatusCode(), but reads the content from request before throws
        /// </summary>
        public static async ValueTask EnsureSuccess(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
#if NET5_0_OR_GREATER
                throw new HttpRequestException(content, new Exception(response.ReasonPhrase), response.StatusCode);
#else
                throw new HttpRequestException(content);
#endif
            }
        }
    }
}
