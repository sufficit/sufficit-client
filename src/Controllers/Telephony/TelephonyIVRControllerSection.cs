using Microsoft.Extensions.Logging;
using Sufficit.EndPoints;
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
    public sealed class TelephonyIVRControllerSection : AuthenticatedControllerSection
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

        /// <summary>
        /// Get single IVR by search parameters
        /// </summary>
        public Task<IVR?> Find(IVRSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("find by parameters: {?}", parameters);

            var uri = new Uri($"{Controller}{Prefix}/Find", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return Request<IVR>(message, cancellationToken);
        }

        /// <summary>
        /// Remove / Delete a single IVR
        /// </summary>
        public Task Remove(Guid ivrId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("remove ivr: {ivrid}", ivrId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["ivrid"] = ivrId.ToString();

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        /// <summary>
        /// Add or Update an IVR
        /// </summary>
        public Task<EndPointResponse<IVR>?> AddOrUpdate(IVR ivr, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("add or update ivr: {id}", ivr.Id);

            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(ivr, null, _json);
            return Request<EndPointResponse<IVR>>(message, cancellationToken);
        }

        /// <summary>
        /// Search IVRs with parameters
        /// </summary>
        public Task<IEnumerable<IVR>> Search(IVRSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("search by parameters: {?}", parameters);

            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<IVR>(message, cancellationToken);
        }

        /// <summary>
        /// Get IVR options
        /// </summary>
        public Task<IEnumerable<AsteriskMenuOption>> GetOptions(Guid ivrId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("options by id: {?}", ivrId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["ivrid"] = ivrId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/options?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<AsteriskMenuOption>(message, cancellationToken);
        }

        /// <summary>
        /// Update IVR options
        /// </summary>
        public Task PostOptions(Guid ivrId, IEnumerable<AsteriskMenuOption>? options, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["ivrid"] = ivrId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/options?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(options, null, _json);
            return Request(message, cancellationToken);
        }
    }
}
