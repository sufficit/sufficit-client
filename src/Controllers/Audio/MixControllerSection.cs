using Microsoft.AspNetCore.Authorization;
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
    public sealed class MixControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = AudioControllerSection.Controller;
        private const string Prefix = "/mix";
        private JsonSerializerOptions _json;

        public MixControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }


        public Task<AudioMixResponse> FromTTS(AudioMixRequestFromTTS parameters, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/fromtts";

            var uri = new Uri(requestEndpoint, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return Request<AudioMixResponse>(message, cancellationToken)!;
        }

        [Authorize]
        public Task Save (AudioMixSaveRequest request, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/save";

            var uri = new Uri(requestEndpoint, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request(message, cancellationToken)!;
        }

        protected override string[]? AnonymousPaths { get; } = { $"{Controller}{Prefix}/fromtts" };
    }
}
