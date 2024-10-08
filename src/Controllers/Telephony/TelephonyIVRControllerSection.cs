﻿using Microsoft.Extensions.Logging;
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
    public sealed class TelephonyIVRControllerSection : ControllerSection, IVRControllerInterface
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/ivr";

        public TelephonyIVRControllerSection(APIClientService service) : base(service) { }   
   
        public async Task<IEnumerable<IVR>> ByContext(Guid contextId, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("by context: {contextid}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/bycontext?{query}", UriKind.Relative);
            var response = await httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Array.Empty<IVR>();
            
            return await response.Content.ReadFromJsonAsync<IEnumerable<IVR>>(jsonOptions, cancellationToken) ?? Array.Empty<IVR>();
        }

        public Task<IVR?> Find(ClientIVRSearchParameters parameters, CancellationToken cancellationToken = default)
            => Find((IVRSearchParameters)parameters, cancellationToken);

        public async Task<IVR?> Find(IVRSearchParameters parameters, CancellationToken cancellationToken)
        {
            logger.LogTrace("by parameters: {?}", parameters);

            var query = ClientIVRSearchParameters.ToQueryString(parameters);
            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var response = await httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await response.Content.ReadFromJsonAsync<IVR>(jsonOptions, cancellationToken);
        }

        public async Task<IEnumerable<IVROption>> GetOptions(Guid ivrId, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("options by id: {?}", ivrId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["ivrid"] = ivrId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/options?{query}", UriKind.Relative);
            var response = await httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Array.Empty<IVROption>();

            return await response.Content.ReadFromJsonAsync<IEnumerable<IVROption>>(jsonOptions, cancellationToken) 
                ?? Array.Empty<IVROption>();
        }

        public async Task Update(Guid ivrId, IEnumerable<AsteriskMenuOption>? options, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["ivrid"] = ivrId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/options?{query}", UriKind.Relative);
            var response = await httpClient.PostAsJsonAsync(uri, options, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task Update (IVR ivr, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var response = await httpClient.PostAsJsonAsync(uri, ivr, cancellationToken);

            await response.EnsureSuccess(cancellationToken);
        }
    }
}
