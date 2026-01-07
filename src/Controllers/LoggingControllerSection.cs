using Sufficit.Logging;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class LoggingControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/logging";

        private readonly JsonSerializerOptions _json;

        public LoggingControllerSection(IAuthenticatedControllerBase cb) : base(cb) 
        {
            _json = cb.Json;
        }

        public Task<IEnumerable<GenericLog<string>>> GetEventsWithContent(LogSearchParameters parameters, CancellationToken cancellationToken)
        {          
            var uri = new Uri($"{Controller}/events", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<GenericLog<string>>(message, cancellationToken);
        }

        public async Task<IEnumerable<GenericLog<T>>> GetEventsWithContent<T>(LogSearchParameters parameters, CancellationToken cancellationToken) where T : class
        {
            // Get base results with content as string (API always returns content as JSON string)
            var stringResults = await GetEventsWithContent(parameters, cancellationToken);

            // Convert each item by deserializing the content string to type T
            return stringResults.Select(item => item.FromJsonLog<T>(_json));
        }

        protected override string[]? AnonymousPaths { get; } = { $"{Controller}/events" };
    }
}
