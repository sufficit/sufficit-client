using Microsoft.Extensions.Logging;
using Sufficit.Net.Http;
using Sufficit.Telephony.Audio;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

        public Task<IEnumerable<AudioPlaceHolder>> ByContext(Guid contextId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by context: {contextid}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/bycontext?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<AudioPlaceHolder>(message, cancellationToken);
        }

        public Task<AudioPlaceHolder?> Find(AudioSearchParameters parameters, CancellationToken cancellationToken)
        {
            _logger.LogTrace("by parameters: {?}", parameters);

            var query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<AudioPlaceHolder>(message, cancellationToken);
        }

        /// <summary>
        /// Get detailed file information for an audio file by ID
        /// </summary>
        /// <param name="id">Audio file ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Audio file information with format details</returns>
        public Task<AudioFileInfo?> FileInfo(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("getting file info for id: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/fileinfo?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<AudioFileInfo>(message, cancellationToken);
        }
    }
}
