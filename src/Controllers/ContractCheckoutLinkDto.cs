using System;
using System.Collections.Generic;

namespace Sufficit.Client.Controllers;

public sealed class ContractCheckoutLinkDto
{
    public Guid ContractId { get; set; }
    public bool Enabled { get; set; }
    public string? Url { get; set; }
    public IReadOnlyList<ContractCheckoutChargeDto> Charges { get; set; } = Array.Empty<ContractCheckoutChargeDto>();
}

public sealed class ContractCheckoutChargeDto
{
    public Guid Id { get; set; }
    public Guid PeriodId { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ConfirmedReceiptUtc { get; set; }
}
