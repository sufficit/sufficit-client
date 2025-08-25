using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Sufficit.Net.Http;
using Sufficit.Telephony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyDestinationControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/destination";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public TelephonyDestinationControllerSection (IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
            _logger = cb.Logger;
        }

        public async Task<IDestination?> FromAsterisk(string asterisk, CancellationToken cancellationToken)
        {
            _logger.LogTrace("getting destination from asterisk: {?}", asterisk);

            var query = $"asterisk={asterisk}";
            var uri = new Uri($"{Controller}{Prefix}/fromasterisk?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await Request<Destination>(message, cancellationToken);
        }

        // For now, only lookup for EndPoints Usage
        public async Task<IEnumerable<IDestination>> InUse(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogTrace("getting destinations in use by: {?}", id);

            var query = $"id={id}";
            var uri = new Uri($"{Controller}{Prefix}/inuse?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestMany<Destination>(message, cancellationToken);
        }

        [Authorize]
        public async Task<IEnumerable<IDestination>> Search(DestinationSearchParameters parameters, CancellationToken cancellationToken)
        {
            _logger.LogTrace("searching destination by parameters: {?}", parameters);

            var query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}{Prefix}/search?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestMany<Destination>(message, cancellationToken);
        }

        /// <summary>
        /// Checks if a destination is being used across all registered modules
        /// </summary>
        /// <param name="destination">Destination to check (supports both ID and Asterisk lookups)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Complete destination usage report</returns>
        public async Task<IEnumerable<DestinationInUseResult>> InUse(DestinationInUseCheck parameters, CancellationToken cancellationToken)
        {
            _logger.LogTrace("checking destination usage: {?}", parameters);

            var uri = new Uri($"{Controller}{Prefix}/InUse", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return await RequestMany<DestinationInUseResult>(message, cancellationToken);
        }

        /// <summary>
        /// Gets information about all registered destination usage checking modules
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Information about registered modules</returns>
        public async Task<object?> GetInUseModules(CancellationToken cancellationToken)
        {
            _logger.LogTrace("getting destination usage modules information");

            var uri = new Uri($"{Controller}{Prefix}/InUse/Modules", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await Request<object>(message, cancellationToken);
        }

        protected override string[]? AnonymousPaths { get; } = {
            $"{Controller}{Prefix}/fromasterisk",
            $"{Controller}{Prefix}/InUse",
            $"{Controller}{Prefix}/InUse/Modules"
        };
    }
}
