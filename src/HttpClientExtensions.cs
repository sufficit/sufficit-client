using Sufficit.EndPoints.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Client
{
    public static class HttpClientExtensions
    {
        public static HttpClient Configure(this IHttpClientFactory factory, EndPointsAPIOptions options)
            => factory.CreateClient(options.ClientId).Configure(options);

        public static HttpClient Configure(this HttpClient source, EndPointsAPIOptions options)
        {
            source.BaseAddress = new Uri(options.BaseUrl);

            if (options.TimeOut.HasValue)
                source.Timeout = TimeSpan.FromSeconds(options.TimeOut.Value);

            source.DefaultRequestHeaders.Add("User-Agent", "C# API Client");
            return source;
        }
    }
}
