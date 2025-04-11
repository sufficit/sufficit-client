using Sufficit.Exchange;
using Sufficit.Exchange.EMail;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace Sufficit.Client.Controllers
{
    public sealed class ExchangeControllerSection : AuthenticatedControllerSection, IExchangeController
    {
        public const string Controller = "/exchange";
        private readonly JsonSerializerOptions _json;

        public ExchangeControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
            Messages = new ExchangeMessagesControllerSection(cb);
        }
    
        public ExchangeMessagesControllerSection Messages { get; }

        /// <inheritdoc cref="IExchangeController.GetViews(ReadReceiptSearchParameters, CancellationToken) "/>
        public Task<IEnumerable<EMailTrackingInfo>> GetViews (ReadReceiptSearchParameters parameters, CancellationToken cancellationToken)
        {
            var query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}/views?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<EMailTrackingInfo>(message, cancellationToken);
        }

        /// <inheritdoc cref="IExchangeController.UnSubscribe(Guid, CancellationToken) "/>
        public Task UnSubscribe (Guid id, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}/unsubscribe?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            return Request(message, cancellationToken);
        }

        protected override string[]? AnonymousPaths { get; } = { 
            $"{Controller}/views",
            $"{Controller}/unsubscribe"
        };
    }
}
