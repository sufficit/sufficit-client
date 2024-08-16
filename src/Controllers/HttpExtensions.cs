using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public static class HttpExtensions
    {
        /// <summary>
        ///     Nearly the HttpResponseMessage.EnsureSuccessStatusCode(), but reads the content from request before throws
        /// </summary>
        public static async ValueTask EnsureSuccess(this HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            if (!response.IsSuccessStatusCode)
            {
                cancellationToken.ThrowIfCancellationRequested();

#if NET5_0_OR_GREATER
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException(content, new Exception(response.ReasonPhrase), response.StatusCode);
#else
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(content);
#endif
            }
        }
    }
}
