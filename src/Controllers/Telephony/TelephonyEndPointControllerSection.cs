using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Sufficit.Net.Http;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyEndPointControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/endpoint";

        private readonly JsonSerializerOptions _json;

        public TelephonyEndPointControllerSection (IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        public Task<IEnumerable<EndPoint>> GetEndPoints(EndPointSearchParameters parameters, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}{Prefix}/search";

            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<EndPoint>(message, cancellationToken);
        }

        public Task<EndPoint?> GetEndPoint(Guid id, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}{Prefix}/byid";

            var query = $"id={id}";
            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<EndPoint>(message, cancellationToken);
        }

        #region PROPERTY

        [Authorize]
        public Task<EndPointProperty?> GetEndPointProperty(EndPointPropertyRequest parameters, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}{Prefix}/property";

            var query = parameters.ToQueryString();
            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<EndPointProperty>(message, cancellationToken);
        }

        [Authorize]
        public Task PostEndPointProperty(EndPointProperty parameters, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}{Prefix}/property";

            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return Request(message, cancellationToken);
        }

        #endregion
    }
}
