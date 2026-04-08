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
    ///     Authenticated client wrapper for the canonical admin-only routes endpoints.
    /// </summary>
    public sealed class TelephonyRoutesControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/routes";
        private readonly JsonSerializerOptions _json;

        public TelephonyRoutesControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        public Task<IEnumerable<Interconnection>> GetInterconnections(CancellationToken cancellationToken = default)
            => RequestMany<Interconnection>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/interconnections", UriKind.Relative)), cancellationToken);

        public Task<Interconnection?> GetInterconnection(Guid interconnectionId, CancellationToken cancellationToken = default)
            => Request<Interconnection>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/interconnection?interconnectionId={interconnectionId}", UriKind.Relative)), cancellationToken);

        public Task<IEnumerable<InterconnectionRegistrationOperationalStatus>> GetRegistrationStatuses(Guid interconnectionId, CancellationToken cancellationToken = default)
            => RequestMany<InterconnectionRegistrationOperationalStatus>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/registrationstatuses?interconnectionId={interconnectionId}", UriKind.Relative)), cancellationToken);

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

        public Task<IEnumerable<TelephonyDomainAlias>> GetDomainAliases(CancellationToken cancellationToken = default)
            => RequestMany<TelephonyDomainAlias>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/domainaliases", UriKind.Relative)), cancellationToken);

        public Task<TelephonyDomainAlias?> AddOrUpdateDomainAlias(TelephonyDomainAlias item, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/domainalias", UriKind.Relative))
            {
                Content = JsonContent.Create(item, null, _json)
            };
            return Request<TelephonyDomainAlias>(message, cancellationToken);
        }

        public Task RemoveDomainAlias(Guid domainAliasId, CancellationToken cancellationToken = default)
            => Request(new HttpRequestMessage(HttpMethod.Delete, new Uri($"{Controller}{Prefix}/domainalias?domainAliasId={domainAliasId}", UriKind.Relative)), cancellationToken);
    }
}
