using Sufficit.Net.Http;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    /// <summary>
    ///     Authenticated client wrapper for the interconnection experimental endpoints.
    /// </summary>
    public sealed class TelephonyInterconnectionExperimentalControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/interconnectionexperimental";
        private readonly JsonSerializerOptions _json;

        public TelephonyInterconnectionExperimentalControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        public Task<IEnumerable<Interconnection>> GetInterconnections(Guid contextId, CancellationToken cancellationToken = default)
            => RequestMany<Interconnection>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/interconnections?contextId={contextId}", UriKind.Relative)), cancellationToken);

        public Task<Interconnection?> GetInterconnection(Guid interconnectionId, CancellationToken cancellationToken = default)
            => Request<Interconnection>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/interconnection?interconnectionId={interconnectionId}", UriKind.Relative)), cancellationToken);

        public Task<Interconnection?> AddOrUpdateInterconnection(Interconnection item, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/interconnection", UriKind.Relative))
            {
                Content = JsonContent.Create(item, null, _json)
            };
            return Request<Interconnection>(message, cancellationToken);
        }

        public Task RemoveInterconnection(Guid interconnectionId, CancellationToken cancellationToken = default)
            => Request(new HttpRequestMessage(HttpMethod.Delete, new Uri($"{Controller}{Prefix}/interconnection?interconnectionId={interconnectionId}", UriKind.Relative)), cancellationToken);

        public Task<InterconnectionProvisioningPreview?> ProvisioningPreview(Guid interconnectionId, CancellationToken cancellationToken = default)
            => Request<InterconnectionProvisioningPreview>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/provisioningpreview?interconnectionId={interconnectionId}", UriKind.Relative)), cancellationToken);

        public Task<InterconnectionProvisioningSyncResult?> ProvisioningSync(Guid interconnectionId, CancellationToken cancellationToken = default)
            => Request<InterconnectionProvisioningSyncResult>(new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/provisioningsync?interconnectionId={interconnectionId}", UriKind.Relative)), cancellationToken);

        public Task<InterconnectionProvisioningSyncResult?> ProvisioningDeprovision(Guid interconnectionId, CancellationToken cancellationToken = default)
            => Request<InterconnectionProvisioningSyncResult>(new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/provisioningdeprovision?interconnectionId={interconnectionId}", UriKind.Relative)), cancellationToken);

        public Task<InterconnectionTestCallSession?> TestCallStart(InterconnectionTestCallRequest request, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/testcallstart", UriKind.Relative))
            {
                Content = JsonContent.Create(request, null, _json)
            };
            return Request<InterconnectionTestCallSession>(message, cancellationToken);
        }

        public Task<InterconnectionTestCallSession?> TestCallSession(Guid sessionId, CancellationToken cancellationToken = default)
            => Request<InterconnectionTestCallSession>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/testcallsession?sessionId={sessionId}", UriKind.Relative)), cancellationToken);
    }
}
