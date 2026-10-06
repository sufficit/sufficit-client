using Microsoft.AspNetCore.Authorization;
using Sufficit.Net.Http;
using Sufficit.Sales;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed partial class SalesControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/sales";
        private const string ManagementRoles = $"{Sufficit.Sales.SalesManagerRole.NormalizedName},{Sufficit.Identity.ManagerRole.NormalizedName},{Sufficit.Identity.AdministratorRole.NormalizedName}";

        private readonly JsonSerializerOptions _json;

        public SalesControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        /// <summary>Reads the current API permission decision; never creates grants or business effects.</summary>
        [Authorize]
        public Task<SalesIntegrationAccessState?> GetIntegrationPermissions(Guid contextId, CancellationToken cancellationToken)
        {
            if (contextId == Guid.Empty) throw new ArgumentException("An explicit customer context is required.", nameof(contextId));
            return Request<SalesIntegrationAccessState>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Controller}/IntegrationAudit/Permissions?contextId={contextId:D}", UriKind.Relative)), cancellationToken);
        }

        /// <summary>Reads context-scoped commercial transport evidence without private event payloads.</summary>
        [Authorize]
        public Task<SalesIntegrationAuditPage?> GetIntegrationAudit(SalesIntegrationAuditSearch parameters, CancellationToken cancellationToken)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            parameters.Validate();
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextId"] = parameters.ContextId.ToString("D");
            query["afterVersion"] = parameters.AfterVersion.ToString(System.Globalization.CultureInfo.InvariantCulture);
            query["limit"] = parameters.Limit.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (parameters.CorrelationId.HasValue) query["correlationId"] = parameters.CorrelationId.Value.ToString("D");
            return Request<SalesIntegrationAuditPage>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Controller}/IntegrationAudit?{query}", UriKind.Relative)), cancellationToken);
        }

        /// <summary>Requests a new bounded transport retry cycle; the API derives the requester identity.</summary>
        [Authorize]
        public Task<SalesIntegrationRetryReceipt?> RetryIntegrationDelivery(SalesIntegrationRetryRequest request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            request.Validate();
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}/IntegrationRetry", UriKind.Relative));
            message.Content = JsonContent.Create(request, null, _json);
            return Request<SalesIntegrationRetryReceipt>(message, cancellationToken);
        }

        /// <summary>
        /// Sales representative panel: the representative and its commissioned customers with
        /// balance, commitment, active services and inbound entries (aggregates only).
        /// </summary>
        /// <remarks>Requires the representative's balanceview entitlement.</remarks>
        [Authorize]
        public Task<IEnumerable<RepresentativeCustomer>> GetRepresentativeCustomers(Guid representativeId, CancellationToken cancellationToken)
        {
            if (representativeId == Guid.Empty) throw new ArgumentException("A representative is required.", nameof(representativeId));
            return RequestMany<RepresentativeCustomer>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Controller}/Representative/Customers?representativeId={representativeId:D}", UriKind.Relative)), cancellationToken);
        }

        /// <summary>
        /// Representatives (contacts commissioned on current records) for search; <see cref="ClientInformation.Total"/>
        /// is the number of customers of each one. Management roles only.
        /// </summary>
        [Authorize(Roles = ManagementRoles)]
        public Task<IEnumerable<ClientInformation>> GetRepresentatives(string? filter, uint? results, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrWhiteSpace(filter)) query["filter"] = filter;
            if (results.HasValue) query["results"] = results.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
            return RequestMany<ClientInformation>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Controller}/Representative/List?{query}", UriKind.Relative)), cancellationToken);
        }

        #region REPRESENTATIVES PROGRAM (Sales/Representatives)

        private const string Representatives = Controller + "/Representatives";

        /// <summary>Registered representatives (management roles).</summary>
        [Authorize(Roles = ManagementRoles)]
        public Task<IEnumerable<SalesRepresentative>> GetRegisteredRepresentatives(bool includeInactive, CancellationToken cancellationToken)
            => RequestMany<SalesRepresentative>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Representatives}?includeInactive={(includeInactive ? "true" : "false")}", UriKind.Relative)), cancellationToken);

        /// <summary>One registration (management roles, or the representative itself).</summary>
        [Authorize]
        public Task<SalesRepresentative?> GetRegisteredRepresentative(Guid id, CancellationToken cancellationToken)
            => Request<SalesRepresentative>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Representatives}/ById?id={id:D}", UriKind.Relative)), cancellationToken);

        /// <summary>Creates or updates a registration (identifier = contact).</summary>
        [Authorize(Roles = ManagementRoles)]
        public Task<SalesRepresentative?> SaveRepresentative(SalesRepresentative item, CancellationToken cancellationToken)
            => Request<SalesRepresentative>(new HttpRequestMessage(HttpMethod.Put, new Uri(Representatives, UriKind.Relative))
                { Content = JsonContent.Create(item, null, _json) }, cancellationToken);

        /// <summary>Portfolio entries by customer and/or representative.</summary>
        [Authorize]
        public Task<IEnumerable<RepresentativeAssignment>> GetRepresentativeAssignments(Guid? customerId, Guid? representativeId, bool includeEnded, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (customerId.HasValue) query["customerId"] = customerId.Value.ToString("D");
            if (representativeId.HasValue) query["representativeId"] = representativeId.Value.ToString("D");
            query["includeEnded"] = includeEnded ? "true" : "false";
            return RequestMany<RepresentativeAssignment>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Representatives}/Assignments?{query}", UriKind.Relative)), cancellationToken);
        }

        /// <summary>Assigns or replaces a representative (replacing requires the in-progress decision).</summary>
        [Authorize(Roles = ManagementRoles)]
        public Task<RepresentativeAssignment?> ChangeRepresentativeAssignment(RepresentativeAssignmentChange change, CancellationToken cancellationToken)
            => Request<RepresentativeAssignment>(new HttpRequestMessage(HttpMethod.Post, new Uri($"{Representatives}/Assignments", UriKind.Relative))
                { Content = JsonContent.Create(change, null, _json) }, cancellationToken);

        /// <summary>Ends an assignment without replacement (history is kept).</summary>
        [Authorize(Roles = ManagementRoles)]
        public Task<RepresentativeAssignment?> EndRepresentativeAssignment(Guid id, string? reason, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString("D");
            if (!string.IsNullOrWhiteSpace(reason)) query["reason"] = reason;
            return Request<RepresentativeAssignment>(new HttpRequestMessage(HttpMethod.Delete,
                new Uri($"{Representatives}/Assignments?{query}", UriKind.Relative)), cancellationToken);
        }

        /// <summary>Commission policies.</summary>
        [Authorize(Roles = ManagementRoles)]
        public Task<IEnumerable<CommissionPolicy>> GetCommissionPolicies(bool includeInactive, CancellationToken cancellationToken)
            => RequestMany<CommissionPolicy>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Representatives}/Policies?includeInactive={(includeInactive ? "true" : "false")}", UriKind.Relative)), cancellationToken);

        [Authorize(Roles = ManagementRoles)]
        public Task<CommissionPolicy?> SaveCommissionPolicy(CommissionPolicy item, CancellationToken cancellationToken)
            => Request<CommissionPolicy>(new HttpRequestMessage(HttpMethod.Put, new Uri($"{Representatives}/Policies", UriKind.Relative))
                { Content = JsonContent.Create(item, null, _json) }, cancellationToken);

        #endregion

        [Authorize]
        public Task<IEnumerable<ClientInformation>> GetClients(string? filter, uint? results, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/clients";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrWhiteSpace(filter))
                query["filter"] = filter;

            if (results.HasValue)
                query["results"] = results.Value.ToString();

            var uri = new Uri($"{ requestEndpoint }?{ query }", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ClientInformation>(message, cancellationToken);
        }


        [Authorize(Roles = ManagementRoles)]
        public Task<IEnumerable<Contract>> GetContracts(ContractSearchParameters parameters, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/contract/search";
            var query = parameters.ToQueryString();
            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<Contract>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<ServiceQuotaProfile?> GetServiceQuotaProfile(Guid catalogItemId, CancellationToken cancellationToken)
            => Request<ServiceQuotaProfile>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Controller}/quotacheckout/profile?catalogItemId={catalogItemId:D}", UriKind.Relative)), cancellationToken);

        [Authorize(Roles = ManagementRoles)]
        public Task<ServiceQuotaProfile?> SaveServiceQuotaProfile(ServiceQuotaProfile profile, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}/quotacheckout/profile", UriKind.Relative));
            message.Content = JsonContent.Create(profile, null, _json);
            return Request<ServiceQuotaProfile>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<IEnumerable<RecurringQuotaChargeDto>> GetRecurringQuotaCharges(Guid contractId, CancellationToken cancellationToken)
            => RequestMany<RecurringQuotaChargeDto>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Controller}/quotacheckout/charges?contractId={contractId:D}", UriKind.Relative)), cancellationToken);

        [Authorize(Roles = ManagementRoles)]
        public Task<RecurringQuotaChargeDto?> CreateRecurringQuotaCharge(Guid periodId, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}/quotacheckout/charge", UriKind.Relative));
            message.Content = JsonContent.Create(new RecurringQuotaChargeRequest { PeriodId = periodId }, null, _json);
            return Request<RecurringQuotaChargeDto>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<LegacyBilledRenewalAvailability?> GetBilledRenewalAvailability(Guid id, CancellationToken cancellationToken)
            => Request<LegacyBilledRenewalAvailability>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Controller}/contract/BilledRenewalAvailability?id={id:D}", UriKind.Relative)), cancellationToken);

        [Authorize(Roles = ManagementRoles)]
        public Task<Contract?> GetContract(Guid id, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}/contract?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<Contract>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<ContractCheckoutLinkDto?> GetContractCheckoutLink(Guid contractId, CancellationToken cancellationToken)
            => Request<ContractCheckoutLinkDto>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Controller}/contractcheckout/link?contractId={contractId:D}", UriKind.Relative)), cancellationToken);

        [Authorize(Roles = ManagementRoles)]
        public Task<ContractCheckoutLinkDto?> SetContractCheckoutLink(Guid contractId, bool enabled, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Post,
                new Uri($"{Controller}/contractcheckout/link", UriKind.Relative));
            message.Content = JsonContent.Create(new { contractId, enabled }, null, _json);
            return Request<ContractCheckoutLinkDto>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<Contract?> SaveContract(Contract item, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/contract", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            return Request<Contract>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<ContractAutomationState?> GetContractAutomation(Guid id, CancellationToken cancellationToken)
            => Request<ContractAutomationState>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Controller}/contract/automation?id={id:D}", UriKind.Relative)), cancellationToken);

        [Authorize(Roles = ManagementRoles)]
        public Task<ContractAutomationState?> SetContractAutomation(ContractAutomationRequest request, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}/contract/automation", UriKind.Relative));
            message.Content = JsonContent.Create(request, null, _json);
            return Request<ContractAutomationState>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task DeleteContract(Guid id, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}/contract?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<IEnumerable<ServiceCatalogItem>> SearchServiceCatalog(
            ServiceCatalogSearchParameters parameters,
            CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/servicecatalog/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<ServiceCatalogItem>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<ServiceCatalogItem?> GetServiceCatalogItem(Guid id, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            var uri = new Uri($"{Controller}/servicecatalog?{query}", UriKind.Relative);
            return Request<ServiceCatalogItem>(new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<ServiceCatalogItem?> SaveServiceCatalogItem(
            ServiceCatalogItem item,
            CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/servicecatalog", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            return Request<ServiceCatalogItem>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task DeleteServiceCatalogItem(Guid id, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            var uri = new Uri($"{Controller}/servicecatalog?{query}", UriKind.Relative);
            return Request(new HttpRequestMessage(HttpMethod.Delete, uri), cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<ContractMigrationResult?> PreviewContractMigration(
            ContractMigrationRequest request,
            CancellationToken cancellationToken)
        {
            request.DryRun = true;
            var uri = new Uri($"{Controller}/migrate/contracts/preview", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<ContractMigrationResult>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<ContractMigrationResult?> ApplyContractMigration(
            ContractMigrationRequest request,
            CancellationToken cancellationToken)
        {
            request.DryRun = false;
            var uri = new Uri($"{Controller}/migrate/contracts/apply", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<ContractMigrationResult>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<IEnumerable<ContractPeriod>> GetPeriods(ContractPeriodSearchParameters parameters, CancellationToken cancellationToken)
        {
            var query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}/contract/periods?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ContractPeriod>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<IEnumerable<ContractInterruption>> GetInterruptions(ContractInterruptionSearchParameters parameters, CancellationToken cancellationToken)
        {
            var query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}/contract/interruptions?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ContractInterruption>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<IEnumerable<ContractAdjustment>> GetAdjustments(ContractAdjustmentSearchParameters parameters, CancellationToken cancellationToken)
        {
            var query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}/contract/adjustments?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ContractAdjustment>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<ContractInterruption?> SaveInterruption(ContractInterruption item, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/contract/interruption", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            return Request<ContractInterruption>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task DeleteInterruption(Guid id, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}/contract/interruption?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<ContractPeriod?> ClosePeriod(ContractPeriodOperationRequest request, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/contract/period/close", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<ContractPeriod>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<ContractPeriod?> ReopenPeriod(ContractPeriodOperationRequest request, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/contract/period/reopen", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<ContractPeriod>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<LegacyMonthlyRenewalAvailability?> GetMonthlyRenewalAvailability(Guid id, CancellationToken cancellationToken)
            => Request<LegacyMonthlyRenewalAvailability>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Controller}/contract/MonthlyRenewalAvailability?id={id:D}", UriKind.Relative)), cancellationToken);

        /// <summary>Returns the persisted successor ID, not confirmation of credit delivery.
        /// On retry reuse the exact expected snapshot from the original request.</summary>
        [Authorize(Roles = ManagementRoles)]
        public Task<Guid?> RenewBilled(LegacyBilledRenewalRequest request, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/contract/RenewBilled", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return RequestStruct<Guid>(message, cancellationToken);
        }

        /// <summary>Generic monthly renewal; retries must retain the original snapshot.</summary>
        [Authorize(Roles = ManagementRoles)]
        public Task<Guid?> RenewMonthly(LegacyMonthlyRenewalRequest request, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}/contract/RenewMonthly", UriKind.Relative));
            message.Content = JsonContent.Create(request, null, _json);
            return RequestStruct<Guid>(message, cancellationToken);
        }

        [Authorize(Roles = ManagementRoles)]
        public Task<ContractPeriod?> ForceRecalculatePeriod(ContractPeriodOperationRequest request, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/contract/period/recalculate", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<ContractPeriod>(message, cancellationToken);
        }
    }
}
