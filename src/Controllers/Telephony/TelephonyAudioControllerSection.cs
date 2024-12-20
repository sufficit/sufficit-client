using Microsoft.Extensions.Logging;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyAudioControllerSection : ControllerSection, AudioControllerInterface
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/audio";

        public TelephonyAudioControllerSection(APIClientService service) : base(service) { }    

        public async Task<IEnumerable<Sufficit.Telephony.Audio>> ByContext(Guid contextId, CancellationToken cancellationToken = default)
        {
            logger.LogTrace("by context: {contextid}", contextId);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}{Prefix}/bycontext?{query}", UriKind.Relative);
            var response = await httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Array.Empty<Sufficit.Telephony.Audio>();
            
            return await response.Content.ReadFromJsonAsync<IEnumerable<Sufficit.Telephony.Audio>>(jsonOptions, cancellationToken) ?? Array.Empty<Sufficit.Telephony.Audio>();
        }

        public async Task<Sufficit.Telephony.Audio?> Find(AudioSearchParameters parameters, CancellationToken cancellationToken)
        {
            logger.LogTrace("by parameters: {?}", parameters);

            var query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var response = await httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await response.Content.ReadFromJsonAsync<Sufficit.Telephony.Audio>(jsonOptions, cancellationToken);
        }
    }
}
