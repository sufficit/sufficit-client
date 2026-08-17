using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Gateway
{
    /// <summary>
    /// Read-only operational history for the Asaas electronic-invoice webhook pipeline.
    /// </summary>
    public sealed class AsaasInvoicesControllerSection : AuthenticatedControllerSection
    {
        private const string HistoryPath = "/Gateway/Asaas/Invoices/History";

        public AsaasInvoicesControllerSection(IAuthenticatedControllerBase cb)
            : base(cb)
        {
        }

        public async Task<IReadOnlyList<AsaasInvoiceWebhookHistoryItem>?> GetHistoryAsync(
            string? invoiceId,
            string? result,
            int limit,
            bool includePayload,
            CancellationToken cancellationToken)
        {
            var query = $"?limit={Math.Min(200, Math.Max(1, limit))}&includePayload={includePayload.ToString().ToLowerInvariant()}";
            if (!string.IsNullOrWhiteSpace(invoiceId))
                query += $"&invoiceId={Uri.EscapeDataString(invoiceId.Trim())}";
            if (!string.IsNullOrWhiteSpace(result))
                query += $"&result={Uri.EscapeDataString(result.Trim().ToLowerInvariant())}";

            return await Request<List<AsaasInvoiceWebhookHistoryItem>>(
                    new HttpRequestMessage(HttpMethod.Get, HistoryPath + query),
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }

    public sealed class AsaasInvoiceWebhookHistoryItem
    {
        public Guid Id { get; set; }
        public string ProviderEventId { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public DateTime EventAtUtc { get; set; }
        public string ProviderInvoiceId { get; set; } = string.Empty;
        public Guid ElectronicInvoiceId { get; set; }
        public string? ProviderStatus { get; set; }
        public DateTime ReceivedAtUtc { get; set; }
        public DateTime? ProcessedAtUtc { get; set; }
        public int AttemptCount { get; set; }
        public string Result { get; set; } = string.Empty;
        public string? Outcome { get; set; }
        public bool Applied { get; set; }
        public string? LastErrorCode { get; set; }
        public string? LastErrorMessage { get; set; }
        public string? Payload { get; set; }
        public IReadOnlyList<AsaasInvoiceWebhookAttempt> Attempts { get; set; }
            = Array.Empty<AsaasInvoiceWebhookAttempt>();
    }

    public sealed class AsaasInvoiceWebhookAttempt
    {
        public int AttemptNumber { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
        public string? Outcome { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public int? HttpStatusCode { get; set; }
    }
}
