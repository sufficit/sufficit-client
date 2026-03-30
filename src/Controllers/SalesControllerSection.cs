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


        [Authorize(Roles = Sufficit.Sales.SalesManagerRole.NormalizedName)]
        public Task<IEnumerable<Contract>> GetContracts(ContractSearchParameters parameters, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/contract/search";
            var query = parameters.ToQueryString();
            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<Contract>(message, cancellationToken);
        }

        [Authorize(Roles = Sufficit.Sales.SalesManagerRole.NormalizedName)]
        public Task<Contract?> GetContract(Guid id, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}/contract?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<Contract>(message, cancellationToken);
        }

        [Authorize(Roles = Sufficit.Sales.SalesManagerRole.NormalizedName)]
        public Task<Contract?> SaveContract(Contract item, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/contract", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            return Request<Contract>(message, cancellationToken);
        }

        [Authorize(Roles = Sufficit.Sales.SalesManagerRole.NormalizedName)]
        public Task<IEnumerable<ContractPeriod>> GetPeriods(ContractPeriodSearchParameters parameters, CancellationToken cancellationToken)
        {
            var query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}/contract/periods?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ContractPeriod>(message, cancellationToken);
        }

        [Authorize(Roles = Sufficit.Sales.SalesManagerRole.NormalizedName)]
        public Task<IEnumerable<ContractInterruption>> GetInterruptions(ContractInterruptionSearchParameters parameters, CancellationToken cancellationToken)
        {
            var query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}/contract/interruptions?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ContractInterruption>(message, cancellationToken);
        }

        [Authorize(Roles = Sufficit.Sales.SalesManagerRole.NormalizedName)]
        public Task<IEnumerable<ContractAdjustment>> GetAdjustments(ContractAdjustmentSearchParameters parameters, CancellationToken cancellationToken)
        {
            var query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}/contract/adjustments?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ContractAdjustment>(message, cancellationToken);
        }

        [Authorize(Roles = Sufficit.Sales.SalesManagerRole.NormalizedName)]
        public Task<ContractInterruption?> SaveInterruption(ContractInterruption item, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/contract/interruption", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            return Request<ContractInterruption>(message, cancellationToken);
        }

        [Authorize(Roles = Sufficit.Sales.SalesManagerRole.NormalizedName)]
        public Task DeleteInterruption(Guid id, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{Controller}/contract/interruption?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        [Authorize(Roles = Sufficit.Sales.SalesManagerRole.NormalizedName)]
        public Task<ContractPeriod?> ClosePeriod(ContractPeriodOperationRequest request, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/contract/period/close", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<ContractPeriod>(message, cancellationToken);
        }

        [Authorize(Roles = Sufficit.Sales.SalesManagerRole.NormalizedName)]
        public Task<ContractPeriod?> ReopenPeriod(ContractPeriodOperationRequest request, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/contract/period/reopen", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<ContractPeriod>(message, cancellationToken);
        }

        [Authorize(Roles = Sufficit.Sales.SalesManagerRole.NormalizedName)]
        public Task<ContractPeriod?> ForceRecalculatePeriod(ContractPeriodOperationRequest request, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/contract/period/recalculate", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request<ContractPeriod>(message, cancellationToken);
        }
    }
}
