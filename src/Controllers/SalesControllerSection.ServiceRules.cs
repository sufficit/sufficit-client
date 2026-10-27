using Microsoft.AspNetCore.Authorization;
using Sufficit.Sales;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    /// <summary>Schedules ("réguas") of notification, cut-off and control steps.</summary>
    public sealed partial class SalesControllerSection
    {
        [Authorize(Roles = ManagementRoles)]
        public Task<IEnumerable<ServiceRule>> SearchServiceRules(ServiceRuleSearchParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/servicerules/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<ServiceRule>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<ServiceRule?> GetServiceRule(Guid id, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            var uri = new Uri($"{Controller}/servicerules?{query}", UriKind.Relative);
            return Request<ServiceRule>(new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken);
        }

        /// <summary>Active schedule of a kind for a contract; contract binding wins over the catalog one.</summary>
        [Authorize(Roles = ManagementRoles)]
        public Task<ServiceRule?> ResolveServiceRule(Guid catalogItemId, Guid? contractId, ServiceRuleKind kind, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["catalogItemId"] = catalogItemId.ToString();
            if (contractId.HasValue) query["contractId"] = contractId.Value.ToString();
            query["kind"] = kind.ToString();
            var uri = new Uri($"{Controller}/servicerules/resolve?{query}", UriKind.Relative);
            return Request<ServiceRule>(new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<IEnumerable<ServiceRuleRevision>> GetServiceRuleRevisions(Guid id, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            var uri = new Uri($"{Controller}/servicerules/revisions?{query}", UriKind.Relative);
            return RequestMany<ServiceRuleRevision>(new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken);
        }

        /// <summary>Saves a new version. A conflict (stale version, second active schedule, invalid steps) carries a user facing message.</summary>
        [Authorize(Roles = ManagementRoles)]
        public Task<ServiceRule?> SaveServiceRule(ServiceRule rule, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/servicerules", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(rule, null, _json);
            return Request<ServiceRule>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task DeleteServiceRule(Guid id, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            var uri = new Uri($"{Controller}/servicerules?{query}", UriKind.Relative);
            return Request(new HttpRequestMessage(HttpMethod.Delete, uri), cancellationToken);
        }
    }
}
