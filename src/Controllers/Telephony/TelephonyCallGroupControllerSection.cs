using Microsoft.Extensions.Logging;
using Sufficit.Json;
using Sufficit.Net.Http;
using Sufficit.Telephony;
using Sufficit.Telephony.Asterisk.RealTime;
using Sufficit.Telephony.Audio;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyCallGroupControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/callgroups";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public TelephonyCallGroupControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
            _logger = cb.Logger;
        }

        #region REALTIME CONFIG

        public Task<IEnumerable<CallGroup>> GetByContext(Guid contextid, CancellationToken cancellationToken)
        {
            _logger.LogTrace("by context id: {?}", contextid);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextid.ToString();

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<CallGroup>(message, cancellationToken)!;
        }

        public Task AddOrUpdate(CallGroup item, CancellationToken cancellationToken)
        {
            _logger.LogTrace("callgroup add or update: {?}", item.ToJsonOrError());

            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            return Request(message, cancellationToken);
        }

        public Task Delete(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogTrace("delete by id: {?}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        #endregion
    }
}
