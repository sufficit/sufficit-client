using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace Sufficit.Client.Controllers;
public sealed class BillingOverviewRowDto
{
    public Guid Id { get; set; }
    public Guid ContextId { get; set; }
    public Guid? ContractId { get; set; }
    public string? CustomerTitle { get; set; }
    public string Title { get; set; } = "";
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public string Status { get; set; } = "";
    public string Kind { get; set; } = "";
    public bool Simulation { get; set; }
}
public sealed class BillingOverviewPageDto
{
    public List<BillingOverviewRowDto> Items { get; set; } = new();
    public int? NextOffset { get; set; }
    public string Coverage { get; set; } = "";
}
public sealed partial class SalesControllerSection
{
    [Authorize(Roles = ManagementRoles)]
    public Task<BillingOverviewPageDto?> GetBillingOverview(string from, string to, Guid? contextId, Guid? contractId,
        string state, string kind, int offset, CancellationToken ct)
        => Request<BillingOverviewPageDto>(new HttpRequestMessage(HttpMethod.Get, new Uri(
            $"{Controller}/billingcheckout/overview?from={Uri.EscapeDataString(from)}&to={Uri.EscapeDataString(to)}&state={Uri.EscapeDataString(state)}&kind={Uri.EscapeDataString(kind)}&offset={offset}"
            + (contextId.HasValue ? $"&contextId={contextId:D}" : "") + (contractId.HasValue ? $"&contractId={contractId:D}" : ""), UriKind.Relative)), ct);
}
