using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Sufficit.Net.Http;
using Sufficit.Telephony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyDestinationControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/destination";

        private readonly ILogger _logger;

        public TelephonyDestinationControllerSection (IAuthenticatedControllerBase cb) : base(cb)
        {
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

        protected override string[]? AnonymousPaths { get; } = {
            $"{Controller}{Prefix}/fromasterisk",
            $"{Controller}{Prefix}/inuse"
        };
    }
}
