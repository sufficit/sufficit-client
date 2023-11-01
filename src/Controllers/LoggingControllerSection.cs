using Sufficit.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class LoggingControllerSection : ControllerSection
    {
        public const string Controller = "/logging";

        public LoggingControllerSection(APIClientService service) : base(service) { }

        public Task<IEnumerable<JsonLog>> GetEventsWithContent(LogSearchParameters parameters, CancellationToken cancellationToken)
        {          
            var uri = new Uri($"{Controller}/events", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return RequestMany<JsonLog>(message, cancellationToken);
        }
    }
}
