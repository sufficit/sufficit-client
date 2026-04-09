using Sufficit.Net.Http;
using Sufficit.Telephony.Trunk;
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
    ///     Authenticated client wrapper for the telephony trunk catalog.
    /// </summary>
    public sealed class TelephonyTrunkControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/trunk";
        private readonly JsonSerializerOptions _json;

        public TelephonyTrunkControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        public Task<IEnumerable<Trunk>> GetTrunks(Guid? contextId = null, CancellationToken cancellationToken = default)
        {
            var uri = contextId.HasValue && contextId.Value != Guid.Empty
                ? $"{Controller}{Prefix}?contextId={contextId.Value}"
                : $"{Controller}{Prefix}";

            return RequestMany<Trunk>(new HttpRequestMessage(HttpMethod.Get, new Uri(uri, UriKind.Relative)), cancellationToken);
        }

        public Task<Trunk?> GetTrunk(Guid trunkId, CancellationToken cancellationToken = default)
            => Request<Trunk>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/single?trunkId={trunkId}", UriKind.Relative)), cancellationToken);

        public Task<Trunk?> AddOrUpdateTrunk(Trunk item, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/single", UriKind.Relative))
            {
                Content = JsonContent.Create(item, null, _json)
            };

            return Request<Trunk>(message, cancellationToken);
        }

        public Task RemoveTrunk(Guid trunkId, CancellationToken cancellationToken = default)
            => Request(new HttpRequestMessage(HttpMethod.Delete, new Uri($"{Controller}{Prefix}/single?trunkId={trunkId}", UriKind.Relative)), cancellationToken);

        public Task<TrunkProvisioningPreview?> ProvisioningPreview(Guid trunkId, CancellationToken cancellationToken = default)
            => Request<TrunkProvisioningPreview>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/provisioningpreview?trunkId={trunkId}", UriKind.Relative)), cancellationToken);

        public Task<TrunkOperationalStatus?> OperationalStatus(Guid trunkId, CancellationToken cancellationToken = default)
            => Request<TrunkOperationalStatus>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/operationalstatus?trunkId={trunkId}", UriKind.Relative)), cancellationToken);

        public Task<TrunkProvisioningSyncResult?> ProvisioningSync(Guid trunkId, CancellationToken cancellationToken = default)
            => Request<TrunkProvisioningSyncResult>(new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/provisioningsync?trunkId={trunkId}", UriKind.Relative)), cancellationToken);

        public Task<TrunkProvisioningSyncResult?> ProvisioningDeprovision(Guid trunkId, CancellationToken cancellationToken = default)
            => Request<TrunkProvisioningSyncResult>(new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/provisioningdeprovision?trunkId={trunkId}", UriKind.Relative)), cancellationToken);
    }
}
