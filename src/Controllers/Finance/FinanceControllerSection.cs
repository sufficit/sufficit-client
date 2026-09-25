using Sufficit;
using Sufficit.Contacts;
using Sufficit.Finance;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Finance
{
    public sealed partial class FinanceControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/finance";

        private readonly JsonSerializerOptions _json;

        public FinanceControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
            LegacyBankSlip = new LegacyBankSlipControllerSection(cb);
            BankSlip = new BankSlipControllerSection(cb);
            ElectronicInvoice = new ElectronicInvoiceControllerSection(cb);
        }

        public LegacyBankSlipControllerSection LegacyBankSlip { get; }

        public BankSlipControllerSection BankSlip { get; }

        public ElectronicInvoiceControllerSection ElectronicInvoice { get; }

        public Task<decimal?> GetBalance(Guid contextId, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}/balance?contextId={contextId:D}", UriKind.Relative);
            return RequestStruct<decimal>(new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken);
        }

        public Task<IEnumerable<Record>> SearchRecords(
            RecordSearchParameters parameters,
            CancellationToken cancellationToken = default)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (parameters.ContextId.HasValue)
                query[nameof(parameters.ContextId)] = parameters.ContextId.Value.ToString("D");
            if (parameters.UserId.HasValue)
                query[nameof(parameters.UserId)] = parameters.UserId.Value.ToString("D");
            if (parameters.Limit.HasValue)
                query[nameof(parameters.Limit)] = parameters.Limit.Value.ToString(CultureInfo.InvariantCulture);
            if (parameters.Skip.HasValue)
                query[nameof(parameters.Skip)] = parameters.Skip.Value.ToString(CultureInfo.InvariantCulture);
            if (parameters.Active.HasValue)
                query[nameof(parameters.Active)] = parameters.Active.Value.ToString().ToLowerInvariant();
            if (parameters.Start.HasValue)
                query[nameof(parameters.Start)] = ToQueryDate(parameters.Start.Value);
            if (parameters.End.HasValue)
                query[nameof(parameters.End)] = ToQueryDate(parameters.End.Value);

            AddTextFilter(query, nameof(parameters.Kind), parameters.Kind);
            AddTextFilter(query, nameof(parameters.Description), parameters.Description);
            AddTextFilter(query, nameof(parameters.Document), parameters.Document);

            if (parameters.Timestamp != null)
            {
                if (parameters.Timestamp.Exact.HasValue)
                    query[$"{nameof(parameters.Timestamp)}.{nameof(parameters.Timestamp.Exact)}"] =
                        ToQueryDate(parameters.Timestamp.Exact.Value);
                if (parameters.Timestamp.Start.HasValue)
                    query[$"{nameof(parameters.Timestamp)}.{nameof(parameters.Timestamp.Start)}"] =
                        ToQueryDate(parameters.Timestamp.Start.Value);
                if (parameters.Timestamp.End.HasValue)
                    query[$"{nameof(parameters.Timestamp)}.{nameof(parameters.Timestamp.End)}"] =
                        ToQueryDate(parameters.Timestamp.End.Value);

                query[$"{nameof(parameters.Timestamp)}.{nameof(parameters.Timestamp.Inclusive)}"] =
                    parameters.Timestamp.Inclusive.ToString().ToLowerInvariant();
            }

            var uri = new Uri($"{Controller}/record/search?{query}", UriKind.Relative);
            return RequestMany<Record>(new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken);
        }

        public Task<RecentPaymentsResult?> GetRecentPayments(
            RecentPaymentSearchParameters parameters,
            CancellationToken cancellationToken = default)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (parameters.Start.HasValue)
                query[nameof(parameters.Start)] = ToQueryDate(parameters.Start.Value);
            if (parameters.End.HasValue)
                query[nameof(parameters.End)] = ToQueryDate(parameters.End.Value);
            query[nameof(parameters.MinimumValue)] = parameters.MinimumValue.ToString(CultureInfo.InvariantCulture);
            if (parameters.Limit.HasValue)
                query[nameof(parameters.Limit)] = parameters.Limit.Value.ToString(CultureInfo.InvariantCulture);
            if (parameters.Paging != null)
            {
                query["Paging.Position"] = parameters.Paging.Position.ToString(CultureInfo.InvariantCulture);
                query["Paging.Size"] = parameters.Paging.Size.ToString(CultureInfo.InvariantCulture);
            }
            query[nameof(parameters.IncludeBankSlip)] = parameters.IncludeBankSlip.ToString().ToLowerInvariant();
            query[nameof(parameters.IncludeCard)] = parameters.IncludeCard.ToString().ToLowerInvariant();
            query[nameof(parameters.IncludeMercadoPago)] = parameters.IncludeMercadoPago.ToString().ToLowerInvariant();

            var uri = new Uri($"{Controller}/payment/recent?{query}", UriKind.Relative);
            return Request<RecentPaymentsResult>(new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken);
        }

        #region TRANSFERS

        /// <summary>
        ///     Searches financial transfers between cost centers.
        /// </summary>
        public Task<IEnumerable<BalanceTransferExtended>> SearchTransfers(
            FinanceTransferSearchParameters parameters,
            CancellationToken cancellationToken = default)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var uri = new Uri($"{Controller}/transfers/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(parameters, options: _json)
            };

            return RequestMany<BalanceTransferExtended>(message, cancellationToken);
        }

        /// <summary>
        ///     Gets one financial transfer by its identifier.
        /// </summary>
        public Task<BalanceTransferExtended?> GetTransfer(Guid id, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}/transfer/{id:D}", UriKind.Relative);
            return Request<BalanceTransferExtended>(new HttpRequestMessage(HttpMethod.Get, uri), cancellationToken);
        }

        /// <summary>
        ///     Transfers value between two financial cost centers.
        /// </summary>
        public Task<BalanceTransferExtended?> Transfer(
            FinanceTransferRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var uri = new Uri($"{Controller}/transfer", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request, options: _json)
            };

            return Request<BalanceTransferExtended>(message, cancellationToken);
        }

        /// <summary>
        ///     Reverts a financial transfer. Both records are kept so the audit trail survives,
        ///     which is what separates it from the legacy cancel that deleted them.
        /// </summary>
        public Task<BalanceTransferExtended?> RevertTransfer(Guid id, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}/transfer/{id:D}", UriKind.Relative);
            return Request<BalanceTransferExtended>(new HttpRequestMessage(HttpMethod.Delete, uri), cancellationToken);
        }

        #endregion

        public Task<IdTitlePair?> GetEntity(Guid id, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}/entity?id={id:N}", UriKind.Relative));
            return Request<IdTitlePair>(message, cancellationToken);
        }

        public Task<IdTitlePair?> GetEntity(string document, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(document)] = document;
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}/entity?{query}", UriKind.Relative));
            return Request<IdTitlePair>(message, cancellationToken);
        }

        private static void AddTextFilter(
            System.Collections.Specialized.NameValueCollection query,
            string name,
            TextFilter? filter)
        {
            if (filter?.IsValid != true)
                return;

            query[$"{name}.{nameof(filter.Text)}"] = filter.Text;
            query[$"{name}.{nameof(filter.ExactMatch)}"] =
                filter.ExactMatch.ToString().ToLowerInvariant();
        }

        private static string ToQueryDate(DateTime value)
            => value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
    }
}
