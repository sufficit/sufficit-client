using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.Net.Http;
using Sufficit.Telephony;
using Sufficit.Telephony.Asterisk;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyIVRControllerSection : AuthenticatedControllerSection, IVRControllerInterface
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/ivr";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public TelephonyIVRControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        public Task<IEnumerable<IVR>> ByContext(Guid contextId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by context: {contextid}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/bycontext?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<IVR>(message, cancellationToken);
        }

        public Task<IVR?> Find(ClientIVRSearchParameters parameters, CancellationToken cancellationToken = default)
            => Find((IVRSearchParameters)parameters, cancellationToken);

        public Task<IVR?> Find(IVRSearchParameters parameters, CancellationToken cancellationToken)
        {
            _logger.LogTrace("by parameters: {?}", parameters);

            var query = ClientIVRSearchParameters.ToQueryString(parameters);
            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<IVR>(message, cancellationToken);
        }

        public Task<IEnumerable<IVROption>> GetOptions(Guid ivrId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("options by id: {?}", ivrId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["ivrid"] = ivrId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/options?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<IVROption>(message, cancellationToken);
        }

        public Task Update(Guid ivrId, IEnumerable<AsteriskMenuOption>? options, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["ivrid"] = ivrId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/options?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(options, null, _json);
            return Request(message, cancellationToken);
        }

        public Task Update (IVR ivr, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(ivr, null, _json);
            return Request(message, cancellationToken);
        }
    }
}
