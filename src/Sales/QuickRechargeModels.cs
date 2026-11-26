using System;

namespace Sufficit.Client.Sales
{
    /// <summary>What the "+ Recarga" dialog shows before asking the amount (server answer of Sales/QuickRecharge).</summary>
    public sealed class QuickRechargeOverview
    {
        public Guid CustomerId { get; set; }
        /// <summary>False when the customer cannot receive a quick recharge; <see cref="Reason"/> explains why.</summary>
        public bool Available { get; set; }
        public string? Reason { get; set; }
        public Guid? ContractId { get; set; }
        /// <summary>Usual recharge value (the tariffed trunk value), a suggestion only.</summary>
        public decimal SuggestedAmount { get; set; }
        /// <summary>Customer's financial balance (paid money not yet used).</summary>
        public decimal FinancialBalance { get; set; }
        /// <summary>Current call balance in telephony; null when unavailable.</summary>
        public decimal? CallBalance { get; set; }
        /// <summary>Financial responsible that pays this customer's purchases, when any.</summary>
        public Guid? PayerId { get; set; }
        public string? PayerTitle { get; set; }
        public decimal MaxAmount { get; set; }
    }

    /// <summary>Quick recharge: the manager informs only the amount; OperationId makes retries safe.</summary>
    public sealed class QuickRechargeRequest
    {
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public Guid OperationId { get; set; }
    }

    public sealed class QuickRechargeResult
    {
        public Guid OperationId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpirationUtc { get; set; }
        /// <summary>Ready-to-show confirmation text.</summary>
        public string Message { get; set; } = string.Empty;
    }
}
