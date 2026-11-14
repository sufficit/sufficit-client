using Microsoft.AspNetCore.Authorization;
using Sufficit.Finance;
using Sufficit.Identity;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Finance
{
    /// <summary>
    ///     Fiscal consolidation: curated fiscal services, taker state, received payments split by
    ///     invoice type and NFS-e issuing (Finance/Fiscal). Internal managers only.
    /// </summary>
    [Authorize(Roles = $"{AdministratorRole.NormalizedName},{ManagerRole.NormalizedName}")]
    public sealed class FiscalControllerSection : AuthenticatedControllerSection
    {
        private const string Prefix = FinanceControllerSection.Controller + "/fiscal";
        private readonly JsonSerializerOptions _json;

        public FiscalControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        /// <summary>Curated fiscal services accepted by <see cref="Issue"/>.</summary>
        public Task<IEnumerable<FiscalServiceView>> Services(CancellationToken cancellationToken = default)
            => RequestMany<FiscalServiceView>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Prefix}/services", UriKind.Relative)), cancellationToken);

        /// <summary>Received payments of the customer split into invoice lines by fiscal type.</summary>
        public Task<FiscalReceiptsView?> Receipts(Guid contextId, int? days = null, CancellationToken cancellationToken = default)
        {
            var query = $"contextId={contextId:D}" + (days.HasValue ? $"&days={days.Value}" : string.Empty);
            return Request<FiscalReceiptsView>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Prefix}/receipts?{query}", UriKind.Relative)), cancellationToken);
        }

        /// <summary>Fiscal work queue across all customers (1–90 days).</summary>
        public Task<FiscalPendingView?> Pending(int? days = null, CancellationToken cancellationToken = default)
        {
            var query = days.HasValue ? $"?days={days.Value}" : string.Empty;
            return Request<FiscalPendingView>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Prefix}/pending{query}", UriKind.Relative)), cancellationToken);
        }

        /// <summary>Taker registration at the fiscal provider.</summary>
        public Task<FiscalTakerView?> Taker(Guid contextId, CancellationToken cancellationToken = default)
            => Request<FiscalTakerView>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Prefix}/takers/{contextId:D}", UriKind.Relative)), cancellationToken);

        /// <summary>Copies the CRM address to the provider taker registration.</summary>
        public Task<FiscalTakerView?> SyncTaker(Guid contextId, CancellationToken cancellationToken = default)
            => Request<FiscalTakerView>(new HttpRequestMessage(HttpMethod.Post, new Uri($"{Prefix}/takers/{contextId:D}/sync", UriKind.Relative)), cancellationToken);

        /// <summary>Cancels an invoice of the customer (reason kept in the audit trail).</summary>
        public Task<FiscalIssueResult?> Cancel(Guid contextId, string providerId, string reason, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post,
                new Uri($"{Prefix}/invoices/{Uri.EscapeDataString(providerId)}/cancel?contextId={contextId:D}", UriKind.Relative))
            {
                Content = JsonContent.Create(new FiscalCancelRequest { Reason = reason }, null, _json)
            };
            return Request<FiscalIssueResult>(message, cancellationToken);
        }

        /// <summary>Decides not to issue the invoice of a receipt line (reason required).</summary>
        public Task<FiscalLineDismissal?> Dismiss(FiscalDismissRequest request, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Prefix}/dismiss", UriKind.Relative))
            {
                Content = JsonContent.Create(request, null, _json)
            };
            return Request<FiscalLineDismissal>(message, cancellationToken);
        }

        /// <summary>Puts a dismissed receipt line back in the queue.</summary>
        public Task<FiscalDismissRequest?> Restore(Guid contextId, string reference, CancellationToken cancellationToken = default)
            => Request<FiscalDismissRequest>(new HttpRequestMessage(HttpMethod.Delete,
                new Uri($"{Prefix}/dismiss?contextId={contextId:D}&reference={Uri.EscapeDataString(reference)}", UriKind.Relative)), cancellationToken);

        /// <summary>Schedules one NFS-e (idempotent by reference or payment).</summary>
        public Task<FiscalIssueResult?> Issue(FiscalIssueRequest request, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Prefix}/issue", UriKind.Relative))
            {
                Content = JsonContent.Create(request, null, _json)
            };
            return Request<FiscalIssueResult>(message, cancellationToken);
        }
    }
}
