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
    ///     Authenticated client wrapper for the outbound telephony endpoints.
    /// </summary>
    public sealed class TelephonyOutboundControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/outbound";
        private readonly JsonSerializerOptions _json;

        public TelephonyOutboundControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        /// <summary>
        ///     Lists outbound service assignments for one context.
        /// </summary>
        public Task<IEnumerable<OutboundServiceAssignment>> GetAssignments(Guid contextId, CancellationToken cancellationToken = default)
            => RequestMany<OutboundServiceAssignment>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/serviceassignments?contextId={contextId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Imports a disabled snapshot of the legacy outbound catalog into the assignment and route-rule tables.
        /// </summary>
        public Task<OutboundLegacyImportResult?> ImportLegacyAssignments(Guid contextId, CancellationToken cancellationToken = default)
            => Request<OutboundLegacyImportResult>(new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/importlegacyassignments?contextId={contextId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Lists imported outbound route rules for one assignment.
        /// </summary>
        public Task<IEnumerable<OutboundRouteRule>> GetRouteRules(Guid assignmentId, CancellationToken cancellationToken = default)
            => RequestMany<OutboundRouteRule>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/routerules?assignmentId={assignmentId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Creates or updates one imported outbound route rule row.
        /// </summary>
        public Task<OutboundRouteRule?> AddOrUpdateRouteRule(OutboundRouteRule item, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/routerule", UriKind.Relative))
            {
                Content = JsonContent.Create(item, null, _json)
            };
            return Request<OutboundRouteRule>(message, cancellationToken);
        }

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

    }
}

