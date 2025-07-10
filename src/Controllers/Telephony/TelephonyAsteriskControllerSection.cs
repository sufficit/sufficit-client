using Microsoft.Extensions.Logging;
using Sufficit.Json;
using Sufficit.Net.Http;
using Sufficit.Telephony.Asterisk.RealTime;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyAsteriskControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/asterisk";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public TelephonyAsteriskControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
            _logger = cb.Logger;
        }

        #region REALTIME CONFIG

        public Task<GenericSearchResponse<RealTimeConfig>> Search(RealTimeConfigSearchParameters parameters, CancellationToken cancellationToken)
        {
            _logger.LogTrace("by parameters: {?}", parameters);

            var uri = new Uri($"{Controller}{Prefix}/rtconfig/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return Request<GenericSearchResponse<RealTimeConfig>>(message, cancellationToken)!;
        }

        public Task AddOrUpdate(RealTimeConfig item, CancellationToken cancellationToken)
        {
            _logger.LogTrace("add or update: {?}", item.ToJsonOrError());

            var uri = new Uri($"{Controller}{Prefix}/rtconfig", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            return Request(message, cancellationToken);
        }

        public Task Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogTrace("delete by id: {?}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/rtconfig?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        #endregion
    }
}
