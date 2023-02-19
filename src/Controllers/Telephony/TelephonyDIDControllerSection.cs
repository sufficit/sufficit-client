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
    public sealed class TelephonyDIDControllerSection : ControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/did";

        public TelephonyDIDControllerSection(APIClientService service) : base(service) { }  

        public async Task<IEnumerable<DirectInwardDialing>> ByContext(Guid contextId, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("by context: {contextid}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/bycontext?{query}", UriKind.Relative);
            var response = await httpClient.GetAsync(uri, cancellationToken);
            await response.EnsureSuccess();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Array.Empty<DirectInwardDialing>();
            
            return await response.Content.ReadFromJsonAsync<IEnumerable<DirectInwardDialing>>(jsonOptions, cancellationToken) ?? Array.Empty<DirectInwardDialing>();
        }

        public async Task<DirectInwardDialing?> ById(Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("by id: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/byid?{query}", UriKind.Relative);
            var response = await httpClient.GetAsync(uri, cancellationToken);
            await response.EnsureSuccess();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await response.Content.ReadFromJsonAsync<DirectInwardDialing>(jsonOptions, cancellationToken);
        }

        /// <summary>
        /// Update Owner information
        /// </summary>
        public async Task Owner(OwnerUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/owner", UriKind.Relative);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = JsonContent.Create(parameters, null, jsonOptions);

            var response = await httpClient.SendAsync(request, cancellationToken);
            await response.EnsureSuccess();
        }

        /// <summary>
        /// Update Owner information
        /// </summary>
        public async Task Context(ContextUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/context", UriKind.Relative);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = JsonContent.Create(parameters, null, jsonOptions);

            var response = await httpClient.SendAsync(request, cancellationToken);
            await response.EnsureSuccess();
        }

        /// <summary>
        /// Update Filter information
        /// </summary>
        public async Task Filter(FilterUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/filter", UriKind.Relative);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = JsonContent.Create(parameters, null, jsonOptions);

            var response = await httpClient.SendAsync(request, cancellationToken);
            await response.EnsureSuccess();
        }

        /// <summary>
        /// Update Extra information
        /// </summary>
        public async Task Extra(ExtraUpdateParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/extra", UriKind.Relative);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = JsonContent.Create(parameters, null, jsonOptions);

            var response = await httpClient.SendAsync(request, cancellationToken);
            await response.EnsureSuccess();
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
            request.Content = JsonContent.Create(destination, null, jsonOptions);

            var response = await httpClient.SendAsync(request, cancellationToken);
            await response.EnsureSuccess();
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
            request.Content = JsonContent.Create(properties, null, jsonOptions);

            var response = await httpClient.SendAsync(request, cancellationToken);
            await response.EnsureSuccess();
        }
    }
}
