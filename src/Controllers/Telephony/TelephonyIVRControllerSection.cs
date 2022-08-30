using Microsoft.Extensions.Logging;
using Sufficit.Telephony;
using Sufficit.Telephony.Asterisk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyIVRControllerSection : IVRControllerInterface
    {
        private static string Controller => TelephonyControllerSection.Controller;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions options;

        public TelephonyIVRControllerSection(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            
            options = new JsonSerializerOptions();
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true));
            options.PropertyNameCaseInsensitive = true;
        }

        public async Task<IEnumerable<IVR>> ByContext(Guid contextId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by context: {contextid}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}/ivr/bycontext?{query}", UriKind.Relative);
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Array.Empty<IVR>();
            
            return await response.Content.ReadFromJsonAsync<IEnumerable<IVR>>(options, cancellationToken) ?? Array.Empty<IVR>();
        }

        public Task<IVR?> Find(ClientIVRSearchParameters parameters, CancellationToken cancellationToken = default)
            => Find((IVRSearchParameters)parameters, cancellationToken);

        public async Task<IVR?> Find(IVRSearchParameters parameters, CancellationToken cancellationToken)
        {
            _logger.LogTrace("by parameters: {?}", parameters);

            var query = ClientIVRSearchParameters.ToQueryString(parameters);
            var uri = new Uri($"{Controller}/ivr?{query}", UriKind.Relative);
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await response.Content.ReadFromJsonAsync<IVR>(options, cancellationToken);
        }

        public async Task<IEnumerable<IVROption>> GetOptions(Guid ivrId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("options by id: {?}", ivrId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["ivrid"] = ivrId.ToString();

            var uri = new Uri($"{Controller}/ivr/options?{query}", UriKind.Relative);
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Array.Empty<IVROption>();

            return await response.Content.ReadFromJsonAsync<IEnumerable<IVROption>>(options, cancellationToken) 
                ?? Array.Empty<IVROption>();
        }

        public async Task Update(Guid ivrId, IEnumerable<AsteriskMenuOption>? options, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["ivrid"] = ivrId.ToString();

            var uri = new Uri($"{Controller}/ivr/options?{query}", UriKind.Relative);
            var response = await _httpClient.PostAsJsonAsync(uri, options, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task Update(IVR ivr, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}/ivr", UriKind.Relative);
            var response = await _httpClient.PostAsJsonAsync(uri, ivr, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
