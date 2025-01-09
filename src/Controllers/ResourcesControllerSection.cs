using Sufficit.Client.Controllers.Telephony;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class ResourcesControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/resources";

        public ResourcesControllerSection (IAuthenticatedControllerBase cb) : base(cb)
        {
            TTS = new TTSControllerSection(cb);
        }

        public TTSControllerSection TTS { get; }

        public Task<byte[]?> HTMLToPDF(string html, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}/htmltopdf", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = new StringContent(html, System.Text.Encoding.UTF8, "text/html");
            return RequestBytes(message, cancellationToken);
        }

        protected override string[]? AnonymousPaths { get; } = { 
            $"{Controller}/htmltopdf",
            $"{Controller}/urltopdf" 
        };
    }
}