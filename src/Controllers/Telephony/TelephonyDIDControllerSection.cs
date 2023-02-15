using Microsoft.Extensions.Logging;
using Sufficit.Telephony;
using Sufficit.Telephony.Asterisk;
using Sufficit.Telephony.DIDs;
using Sufficit.Telephony.EventsPanel;
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
        private static string Prefix => "/did";
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

        public async Task<IEnumerable<DirectInwardDialing>> ByContext(Guid contextId, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by context: {contextid}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/bycontext?{query}", UriKind.Relative);
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Array.Empty<DirectInwardDialing>();
            
            return await response.Content.ReadFromJsonAsync<IEnumerable<DirectInwardDialing>>(options, cancellationToken) ?? Array.Empty<DirectInwardDialing>();
        }

        public async Task<DirectInwardDialing?> ById(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("by id: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/byid?{query}", UriKind.Relative);
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await response.Content.ReadFromJsonAsync<DirectInwardDialing>(options, cancellationToken);
        }

        /// <summary>
        /// Update Owner information
        /// </summary>
        public async Task Owner(OwnerUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/owner", UriKind.Relative);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = JsonContent.Create(parameters, null, options);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Update Owner information
        /// </summary>
        public async Task Context(ContextUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/context", UriKind.Relative);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = JsonContent.Create(parameters, null, options);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Update Filter information
        /// </summary>
        public async Task Filter(FilterUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/filter", UriKind.Relative);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = JsonContent.Create(parameters, null, options);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Update Extra information
        /// </summary>
        public async Task Extra(ExtraUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/extra", UriKind.Relative);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = JsonContent.Create(parameters, null, options);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Update Destination information
        /// </summary>
        public async Task Destination(Guid id, DestinationBase destination, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/destination?{query}", UriKind.Relative);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = JsonContent.Create(destination, null, options);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Update basic properties
        /// </summary>
        public async Task Properties(Guid id, DirectInwardDialingProperties properties, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/properties?{query}", UriKind.Relative);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = JsonContent.Create(properties, null, options);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
