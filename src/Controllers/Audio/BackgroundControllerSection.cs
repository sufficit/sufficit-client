using Sufficit.Audio;
using Sufficit.Net.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Audio
{
    public sealed class BackgroundControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = AudioControllerSection.Controller;
        private const string Prefix = "/background";
        private readonly HttpClient _client;
        public BackgroundControllerSection(IAuthenticatedControllerBase cb) : base(cb) 
        {
            _client = cb.Client;
        }

        public Task<IEnumerable<BackgroundAudio>> Search(BackgroundAudioSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<BackgroundAudio>(message, cancellationToken);
        }

        public Task<byte[]?> GetBytes(Guid id, CancellationToken cancellationToken = default)
        {
            var url = GetBytesRelativeUrl(id);
            var uri = new Uri(url, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestBytes(message, cancellationToken);
        }

        public string GetBytesUrl(Guid id) =>
            _client.BaseAddress!.AbsoluteUri.TrimEnd('/') + GetBytesRelativeUrl(id);

        public static string GetBytesRelativeUrl(Guid id)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            return $"{Controller}{Prefix}/bytes?{query}";
        }

        protected override string[]? AnonymousPaths { get; } = { 
            $"{Controller}{Prefix}", 
            $"{Controller}{Prefix}/bytes" 
        };
    }
}
