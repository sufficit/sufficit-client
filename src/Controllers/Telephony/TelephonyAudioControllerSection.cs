using Microsoft.Extensions.Logging;
using Sufficit.Net.Http;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyAudioControllerSection : AuthenticatedControllerSection, AudioControllerInterface
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/audio";

        private readonly ILogger _logger;

        public TelephonyAudioControllerSection (IAuthenticatedControllerBase cb) : base(cb) 
        { 
            _logger = cb.Logger;
        }

        public Task<IEnumerable<Sufficit.Telephony.Audio>> ByContext(Guid contextId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by context: {contextid}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/bycontext?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<Sufficit.Telephony.Audio>(message, cancellationToken);
        }

        public Task<Sufficit.Telephony.Audio?> Find(AudioSearchParameters parameters, CancellationToken cancellationToken)
        {
            _logger.LogTrace("by parameters: {?}", parameters);

            var query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<Sufficit.Telephony.Audio>(message, cancellationToken);
        }
    }
}
