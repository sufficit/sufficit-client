using Sufficit.EndPoints;
using Sufficit.Net.Http;
using Sufficit.Resources.TTS;
using Sufficit.Telephony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TTSControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = ResourcesControllerSection.Controller;
        private const string Prefix = "/tts";

        private readonly JsonSerializerOptions _json;

        public TTSControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        public async Task<TTSResponse> Process(TTSRequest request, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}{Prefix}";

            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);

            using var response = await SendAsync(message, cancellationToken);
            await response.EnsureSuccess(cancellationToken);

            var ttsresponse = new TTSResponse();
            ttsresponse.Audio = await response.Content.ReadAsByteArrayAsync();

            var id = response.Headers.GetValues(TTSResponseMeta.HEADER_ID);
            if (id.Any()) ttsresponse.Id = Guid.Parse(id.First());

            var chars = response.Headers.GetValues(TTSResponseMeta.HEADER_CHARACTERS);
            if (chars.Any()) ttsresponse.Characters = uint.Parse(chars.First());

            return ttsresponse;
        }
    }
}
