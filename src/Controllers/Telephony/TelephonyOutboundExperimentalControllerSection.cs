using Sufficit.Net.Http;
using Sufficit.Telephony;
using Sufficit.Telephony.Outbound;
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
    ///     Authenticated client wrapper for the outbound experimental telephony endpoints.
    /// </summary>
    /// <remarks>
    ///     TODO: migrate callers to a canonical telephony service-management controller once the projection model is finalized.
    /// </remarks>
    public sealed class TelephonyOutboundExperimentalControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/outboundexperimental";
        private readonly JsonSerializerOptions _json;

        public TelephonyOutboundExperimentalControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        /// <summary>
        ///     Lists customer-managed outbound trunks for one context.
        /// </summary>
        public Task<IEnumerable<CustomerTrunk>> GetCustomerTrunks(Guid contextId, CancellationToken cancellationToken = default)
            => RequestMany<CustomerTrunk>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/customertrunks?contextId={contextId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Gets one customer-managed outbound trunk.
        /// </summary>
        public Task<CustomerTrunk?> GetCustomerTrunk(Guid trunkId, CancellationToken cancellationToken = default)
            => Request<CustomerTrunk>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/customertrunk?trunkId={trunkId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Creates or updates one customer-managed outbound trunk.
        /// </summary>
        public Task<CustomerTrunk?> AddOrUpdateCustomerTrunk(CustomerTrunk item, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/customertrunk", UriKind.Relative))
            {
                Content = JsonContent.Create(item, null, _json)
            };
            return Request<CustomerTrunk>(message, cancellationToken);
        }

        /// <summary>
        ///     Removes one customer-managed outbound trunk.
        /// </summary>
        public Task RemoveCustomerTrunk(Guid trunkId, CancellationToken cancellationToken = default)
            => Request(new HttpRequestMessage(HttpMethod.Delete, new Uri($"{Controller}{Prefix}/customertrunk?trunkId={trunkId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Lists outbound service assignments for one context.
        /// </summary>
        public Task<IEnumerable<OutboundServiceAssignment>> GetAssignments(Guid contextId, CancellationToken cancellationToken = default)
            => RequestMany<OutboundServiceAssignment>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/serviceassignments?contextId={contextId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Imports a disabled snapshot of the legacy outbound catalog into the experimental assignment and route-rule tables.
        /// </summary>
        public Task<OutboundLegacyImportResult?> ImportLegacyAssignments(Guid contextId, CancellationToken cancellationToken = default)
            => Request<OutboundLegacyImportResult>(new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/importlegacyassignments?contextId={contextId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Lists imported outbound route rules for one assignment.
        /// </summary>
        public Task<IEnumerable<OutboundRouteRule>> GetRouteRules(Guid assignmentId, CancellationToken cancellationToken = default)
            => RequestMany<OutboundRouteRule>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/routerules?assignmentId={assignmentId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Gets one outbound service assignment by id.
        /// </summary>
        public Task<OutboundServiceAssignment?> GetAssignment(Guid assignmentId, CancellationToken cancellationToken = default)
            => Request<OutboundServiceAssignment>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/serviceassignment?assignmentId={assignmentId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Creates or updates one outbound service assignment.
        /// </summary>
        public Task<OutboundServiceAssignment?> AddOrUpdateAssignment(OutboundServiceAssignment item, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/serviceassignment", UriKind.Relative))
            {
                Content = JsonContent.Create(item, null, _json)
            };
            return Request<OutboundServiceAssignment>(message, cancellationToken);
        }

        /// <summary>
        ///     Removes one outbound service assignment.
        /// </summary>
        public Task RemoveAssignment(Guid assignmentId, CancellationToken cancellationToken = default)
            => Request(new HttpRequestMessage(HttpMethod.Delete, new Uri($"{Controller}{Prefix}/serviceassignment?assignmentId={assignmentId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Lists smart DID bindings for one assignment.
        /// </summary>
        public Task<IEnumerable<OutboundServiceDidBinding>> GetDidBindings(Guid assignmentId, CancellationToken cancellationToken = default)
            => RequestMany<OutboundServiceDidBinding>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/didbindings?assignmentId={assignmentId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Creates or updates one smart DID binding.
        /// </summary>
        public Task<OutboundServiceDidBinding?> AddOrUpdateDidBinding(OutboundServiceDidBinding item, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/didbinding", UriKind.Relative))
            {
                Content = JsonContent.Create(item, null, _json)
            };
            return Request<OutboundServiceDidBinding>(message, cancellationToken);
        }

        /// <summary>
        ///     Removes one smart DID binding.
        /// </summary>
        public Task RemoveDidBinding(Guid bindingId, CancellationToken cancellationToken = default)
            => Request(new HttpRequestMessage(HttpMethod.Delete, new Uri($"{Controller}{Prefix}/didbinding?bindingId={bindingId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Lists the default outbound policy profiles.
        /// </summary>
        public Task<IEnumerable<OutboundServicePolicyProfile>> GetPolicyProfiles(CancellationToken cancellationToken = default)
            => RequestMany<OutboundServicePolicyProfile>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/policyprofiles", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Creates or updates one default outbound policy profile.
        /// </summary>
        public Task<OutboundServicePolicyProfile?> AddOrUpdatePolicyProfile(OutboundServicePolicyProfile item, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/policyprofile", UriKind.Relative))
            {
                Content = JsonContent.Create(item, null, _json)
            };
            return Request<OutboundServicePolicyProfile>(message, cancellationToken);
        }

        /// <summary>
        ///     Lists manager overrides for one context.
        /// </summary>
        public Task<IEnumerable<OutboundServicePolicyOverride>> GetPolicyOverrides(Guid contextId, CancellationToken cancellationToken = default)
            => RequestMany<OutboundServicePolicyOverride>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/policyoverrides?contextId={contextId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Creates or updates one manager override.
        /// </summary>
        public Task<OutboundServicePolicyOverride?> AddOrUpdatePolicyOverride(OutboundServicePolicyOverride item, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/policyoverride", UriKind.Relative))
            {
                Content = JsonContent.Create(item, null, _json)
            };
            return Request<OutboundServicePolicyOverride>(message, cancellationToken);
        }

        /// <summary>
        ///     Generates a route preview without provisioning realtime objects.
        /// </summary>
        public Task<OutboundServiceRoutePreview?> Preview(OutboundServiceRoutePreviewRequest request, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/preview", UriKind.Relative))
            {
                Content = JsonContent.Create(request, null, _json)
            };
            return Request<OutboundServiceRoutePreview>(message, cancellationToken);
        }

        /// <summary>
        ///     Generates a provisioning preview for one customer trunk.
        /// </summary>
        public Task<CustomerTrunkProvisioningPreview?> ProvisioningPreview(Guid trunkId, CancellationToken cancellationToken = default)
            => Request<CustomerTrunkProvisioningPreview>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/provisioningpreview?trunkId={trunkId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Resolves the live operational state for one customer trunk.
        /// </summary>
        public Task<CustomerTrunkOperationalStatus?> OperationalStatus(Guid trunkId, CancellationToken cancellationToken = default)
            => Request<CustomerTrunkOperationalStatus>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/operationalstatus?trunkId={trunkId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Applies the generated realtime PJSIP objects for one customer trunk.
        /// </summary>
        public Task<CustomerTrunkProvisioningSyncResult?> ProvisioningSync(Guid trunkId, CancellationToken cancellationToken = default)
            => Request<CustomerTrunkProvisioningSyncResult>(new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/provisioningsync?trunkId={trunkId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Removes namespaced realtime PJSIP objects for one customer trunk.
        /// </summary>
        public Task<CustomerTrunkProvisioningSyncResult?> ProvisioningDeprovision(Guid trunkId, CancellationToken cancellationToken = default)
            => Request<CustomerTrunkProvisioningSyncResult>(new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/provisioningdeprovision?trunkId={trunkId}", UriKind.Relative)), cancellationToken);
    }
}
