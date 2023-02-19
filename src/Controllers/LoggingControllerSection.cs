using Sufficit.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class LoggingControllerSection : ControllerSection
    {
        public const string Controller = "/logging";

        public LoggingControllerSection(APIClientService service) : base(service) { }

        public async Task<IEnumerable<JsonLog>> GetEventsWithContent(LogSearchParameters parameters, CancellationToken cancellationToken)
        {          
            var uri = new Uri($"{Controller}/events", UriKind.Relative);

            var response = await httpClient.PostAsJsonAsync(uri, parameters, cancellationToken);
            await response.EnsureSuccess();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Array.Empty<JsonLog>();

            return await response.Content.ReadFromJsonAsync<IEnumerable<JsonLog>>(jsonOptions, cancellationToken)
                ?? Array.Empty<JsonLog>();
        }
    }
}
