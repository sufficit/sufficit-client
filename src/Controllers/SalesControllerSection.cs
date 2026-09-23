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
    public sealed class SalesControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/sales";
        private const string ManagementRoles = $"{Sufficit.Sales.SalesManagerRole.NormalizedName},{Sufficit.Identity.ManagerRole.NormalizedName},{Sufficit.Identity.AdministratorRole.NormalizedName}";

        private readonly JsonSerializerOptions _json;

        public SalesControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

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
        public Task<ContractPeriod?> ForceRecalculatePeriod(ContractPeriodOperationRequest request, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/contract/period/recalculate", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<ContractPeriod>(message, cancellationToken);
        }
    }
}
