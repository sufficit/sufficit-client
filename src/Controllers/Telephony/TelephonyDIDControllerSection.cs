using Microsoft.Extensions.Logging;
using Sufficit.Telephony;
using Sufficit.Telephony.Asterisk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyDIDControllerSection
    {
        private static string Controller => TelephonyControllerSection.Controller;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions options;

        public TelephonyDIDControllerSection(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            
            options = new JsonSerializerOptions();
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true));
            options.PropertyNameCaseInsensitive = true;
        }

        public async Task<IEnumerable<DirectInwardDialingV1>> ByContext(Guid contextId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by context: {contextid}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}/did/bycontext?{query}", UriKind.Relative);
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Array.Empty<DirectInwardDialingV1>();
            
            return await response.Content.ReadFromJsonAsync<IEnumerable<DirectInwardDialingV1>>(options, cancellationToken) ?? Array.Empty<DirectInwardDialingV1>();
        }

        public async Task<DirectInwardDialingV1?> ById(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by id: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}/did/byid?{query}", UriKind.Relative);
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await response.Content.ReadFromJsonAsync<DirectInwardDialingV1>(options, cancellationToken);
        }
    }
}
