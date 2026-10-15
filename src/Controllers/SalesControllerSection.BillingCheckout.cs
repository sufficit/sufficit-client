using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Sufficit.Sales;

namespace Sufficit.Client.Controllers;

public sealed class BillingChargeItemDto
{
    public Guid ContractId { get; set; }
    public string Title { get; set; } = string.Empty;
    public long AmountCents { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public IReadOnlyList<ServiceCheckoutDetail> Details { get; set; } = Array.Empty<ServiceCheckoutDetail>();
}
public sealed class ChargePaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public int Method { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ConfirmedReceiptUtc { get; set; }
}
public sealed class BillingChargeDto
{
    public Guid Id { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal PaidAmount { get; set; }
    public IReadOnlyList<ChargePaymentDto>? Payments { get; set; }
    public bool Legacy { get; set; }
    public string? CheckoutUrl { get; set; }
    public DateTime? ConfirmedReceiptUtc { get; set; }
    public IReadOnlyList<BillingChargeItemDto> Items { get; set; } = Array.Empty<BillingChargeItemDto>();
}
public sealed class BillingCheckoutLinkDto
{
    public int? PreferredPaymentMethod { get; set; }
    public Guid ContextId { get; set; }
    public Guid? ContractId { get; set; }
    public bool Enabled { get; set; }
    public int DueDay { get; set; }
    public bool RecurringCardEnabled { get; set; }
    public bool RecurringLocked { get; set; }
    public string? Url { get; set; }
    public IReadOnlyList<BillingChargeDto> Charges { get; set; } = Array.Empty<BillingChargeDto>();
}
public sealed partial class SalesControllerSection
{
    [Authorize(Roles = ManagementRoles)]
    public Task<BillingCheckoutLinkDto?> GetBillingCheckoutLink(Guid contextId, Guid? contractId, CancellationToken ct)
        => Request<BillingCheckoutLinkDto>(new HttpRequestMessage(HttpMethod.Get,
            new Uri($"{Controller}/billingcheckout/link?contextId={contextId:D}" + (contractId.HasValue ? $"&contractId={contractId:D}" : ""), UriKind.Relative)), ct);

    [Authorize(Roles = ManagementRoles)]
    public Task<BillingCheckoutLinkDto?> SetBillingCheckoutLink(Guid contextId, Guid? contractId, bool enabled, int dueDay, bool recurringCardEnabled, CancellationToken ct, int? preferredPaymentMethod = null)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}/billingcheckout/link", UriKind.Relative));
        message.Content = JsonContent.Create(new { contextId, contractId, enabled, dueDay, recurringCardEnabled, preferredPaymentMethod }, null, _json);
        return Request<BillingCheckoutLinkDto>(message, ct);
    }
}
