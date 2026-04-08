using Microsoft.Extensions.Logging;
using Sufficit.Net.Http;
using Sufficit.Resources.Fail2Ban;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Resources
{
    public sealed class Fail2BanControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = ResourcesControllerSection.Controller;
        private const string Prefix = "/fail2ban";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public Fail2BanControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        public Task<IEnumerable<Fail2BanBlockedAddress>> Search(Fail2BanBlockedAddressSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("fail2ban search parameters: {parameters}", parameters);

            var uri = new Uri($"{Controller}{Prefix}/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(parameters, null, _json)
            };

            return RequestMany<Fail2BanBlockedAddress>(message, cancellationToken);
        }

        public Task<Fail2BanUnbanResult?> Unban(Fail2BanUnbanRequest request, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/unban", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request, null, _json)
            };

            return Request<Fail2BanUnbanResult>(message, cancellationToken);
        }

        public Task<Fail2BanUnbanResult?> UnbanAll(Fail2BanUnbanAllRequest request, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/unbanall", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request, null, _json)
            };

            return Request<Fail2BanUnbanResult>(message, cancellationToken);
        }
    }
}