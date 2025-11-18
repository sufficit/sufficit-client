using Sufficit.Logging;
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

        public Task<IEnumerable<GenericLog<T>>> GetEventsWithContent<T>(LogSearchParameters parameters, CancellationToken cancellationToken) where T : class
        {
            var uri = new Uri($"{Controller}/events", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<GenericLog<T>>(message, cancellationToken);
        }


        protected override string[]? AnonymousPaths { get; } = { $"{Controller}/events" };
    }
}
