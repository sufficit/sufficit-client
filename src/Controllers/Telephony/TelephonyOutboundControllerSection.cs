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
        public Task<IEnumerable<OutboundServiceAssignment>> GetAssignments(Guid? contextId = null, CancellationToken cancellationToken = default)
        {
            var uri = contextId.HasValue && contextId.Value != Guid.Empty
                ? $"{Controller}{Prefix}/serviceassignments?contextId={contextId.Value}"
                : $"{Controller}{Prefix}/serviceassignments";

            return RequestMany<OutboundServiceAssignment>(new HttpRequestMessage(HttpMethod.Get, new Uri(uri, UriKind.Relative)), cancellationToken);
        }

        /// <summary>
        ///     Lists the canonical Sufficit-owned outbound service catalog.
        /// </summary>
        public Task<IEnumerable<OutboundCatalogService>> GetCatalogServices(CancellationToken cancellationToken = default)
            => RequestMany<OutboundCatalogService>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/catalogservices", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Gets one canonical Sufficit-owned outbound service catalog entry.
        /// </summary>
        public Task<OutboundCatalogService?> GetCatalogService(Guid catalogServiceId, CancellationToken cancellationToken = default)
            => Request<OutboundCatalogService>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/catalogservice?catalogServiceId={catalogServiceId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Creates or updates one canonical Sufficit-owned outbound service catalog entry.
        /// </summary>
        public Task<OutboundCatalogService?> AddOrUpdateCatalogService(OutboundCatalogService item, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/catalogservice", UriKind.Relative))
            {
                Content = JsonContent.Create(item, null, _json)
            };
            return Request<OutboundCatalogService>(message, cancellationToken);
        }

        /// <summary>
        ///     Removes one canonical Sufficit-owned outbound service catalog entry.
        /// </summary>
        public Task RemoveCatalogService(Guid catalogServiceId, CancellationToken cancellationToken = default)
            => Request(new HttpRequestMessage(HttpMethod.Delete, new Uri($"{Controller}{Prefix}/catalogservice?catalogServiceId={catalogServiceId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Lists telephony-side projections of the outbound services currently owned by one customer.
        /// </summary>
        public Task<IEnumerable<OutboundCustomerService>> GetCustomerServices(Guid contextId, CancellationToken cancellationToken = default)
            => RequestMany<OutboundCustomerService>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/customerservices?contextId={contextId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Gets one telephony-side customer service projection row.
        /// </summary>
        public Task<OutboundCustomerService?> GetCustomerService(Guid customerServiceId, CancellationToken cancellationToken = default)
            => Request<OutboundCustomerService>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/customerservice?customerServiceId={customerServiceId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Creates or updates one telephony-side customer service projection row.
        /// </summary>
        public Task<OutboundCustomerService?> AddOrUpdateCustomerService(OutboundCustomerService item, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/customerservice", UriKind.Relative))
            {
                Content = JsonContent.Create(item, null, _json)
            };
            return Request<OutboundCustomerService>(message, cancellationToken);
        }

        /// <summary>
        ///     Removes one telephony-side customer service projection row.
        /// </summary>
        public Task RemoveCustomerService(Guid customerServiceId, CancellationToken cancellationToken = default)
            => Request(new HttpRequestMessage(HttpMethod.Delete, new Uri($"{Controller}{Prefix}/customerservice?customerServiceId={customerServiceId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Lists customer-facing ordered outbound route sources for one context.
        /// </summary>
        public Task<IEnumerable<OutboundRouteSource>> GetRouteSources(Guid contextId, CancellationToken cancellationToken = default)
            => RequestMany<OutboundRouteSource>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/routesources?contextId={contextId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Gets one customer-facing ordered outbound route source row.
        /// </summary>
        public Task<OutboundRouteSource?> GetRouteSource(Guid routeSourceId, CancellationToken cancellationToken = default)
            => Request<OutboundRouteSource>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/routesource?routeSourceId={routeSourceId}", UriKind.Relative)), cancellationToken);

        /// <summary>
        ///     Creates or updates one customer-facing ordered outbound route source row.
        /// </summary>
        public Task<OutboundRouteSource?> AddOrUpdateRouteSource(OutboundRouteSource item, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/routesource", UriKind.Relative))
            {
                Content = JsonContent.Create(item, null, _json)
            };
            return Request<OutboundRouteSource>(message, cancellationToken);
        }

        /// <summary>
        ///     Removes one customer-facing ordered outbound route source row.
        /// </summary>
        public Task RemoveRouteSource(Guid routeSourceId, CancellationToken cancellationToken = default)
            => Request(new HttpRequestMessage(HttpMethod.Delete, new Uri($"{Controller}{Prefix}/routesource?routeSourceId={routeSourceId}", UriKind.Relative)), cancellationToken);

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
        ///     Removes one outbound route rule.
        /// </summary>
        public Task RemoveRouteRule(Guid routeRuleId, CancellationToken cancellationToken = default)
            => Request(new HttpRequestMessage(HttpMethod.Delete, new Uri($"{Controller}{Prefix}/routerule?routeRuleId={routeRuleId}", UriKind.Relative)), cancellationToken);

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

