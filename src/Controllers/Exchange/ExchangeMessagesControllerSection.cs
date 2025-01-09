using Microsoft.AspNetCore.Authorization;
using Sufficit.Exchange;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class ExchangeMessagesControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = ExchangeControllerSection.Controller;
        private const string Prefix = "/messages";

        private readonly JsonSerializerOptions _json;

        public ExchangeMessagesControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        [Authorize(Sufficit.Identity.ManagerRole.NormalizedName)]
        public Task<IEnumerable<MessageDetails>> GetMessages (MessageDetailsSearchParameters parameters, CancellationToken cancellationToken)
        {          
            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);

            var content = JsonContent.Create(parameters, null, _json);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content
            };
            return RequestMany<MessageDetails>(message, cancellationToken);
        }
    }
}
