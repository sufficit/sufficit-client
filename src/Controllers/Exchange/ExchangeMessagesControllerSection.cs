using Microsoft.AspNetCore.Authorization;
using Sufficit.Exchange;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class ExchangeMessagesControllerSection : ControllerSection
    {
        private const string Controller = ExchangeControllerSection.Controller;
        private const string Prefix = "/messages";

        public ExchangeMessagesControllerSection(APIClientService service) : base(service) { }

        [Authorize(Sufficit.Identity.ManagerRole.NormalizedName)]
        public Task<IEnumerable<MessageDetails>> GetMessages (MessageDetailsSearchParameters parameters, CancellationToken cancellationToken)
        {          
            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);

            var content = JsonContent.Create(parameters, null, jsonOptions);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content
            };
            return RequestMany<MessageDetails>(message, cancellationToken);
        }
    }
}
