using System;
using System.Collections.Generic;

namespace Sufficit.Client.Sales
{
    /// <summary>Ledger record linked to a reconciliation row, with the balance reconstructed around it when provable.</summary>
    public sealed class ServiceReconciliationRecordView
    {
        public Guid Id { get; set; }
        public decimal Value { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Active { get; set; }
        public decimal? BalanceBefore { get; set; }
        public decimal? BalanceAfter { get; set; }
    }

    /// <summary>Interval without coverage between two history rows (exclusive end).</summary>
    public sealed class ServiceReconciliationGapView
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public double Seconds { get; set; }
        public bool Future { get; set; }
    }

    /// <summary>One reconciliation row evaluated by the server. Coverage: Projected/Open/Closed or null; Financial: Matched,
    /// Missing, Divergent, InactiveOnly, Future, Free, Unavailable, AwaitingFunding or Gap.</summary>
    public sealed class ServiceReconciliationRowView
    {
        public string Key { get; set; } = string.Empty;
        public Guid EntryId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public decimal Amount { get; set; }
        public DateTime PostingDate { get; set; }
        public string? HistoricalSettlementReason { get; set; }
        public DateTime? RechargeStart { get; set; }
        public DateTime? RechargeEnd { get; set; }
        public bool NoRechargeRequired { get; set; }
        public string? Coverage { get; set; }
        public bool Advance { get; set; }
        public string Financial { get; set; } = string.Empty;
        public DateTime? CoverageStart { get; set; }
        public bool PostingDateDifference { get; set; }
        /// <summary>Tariffed purchase that waited for the customer's money: the day it was actually bought (debit and
        /// credit), later than the planned posting date. Null when bought on its planned day.</summary>
        public DateTime? PurchasedAt { get; set; }
        public List<ServiceReconciliationRecordView> Records { get; set; } = new List<ServiceReconciliationRecordView>();
        public string? Error { get; set; }
        public ServiceReconciliationGapView? Gap { get; set; }
    }

    /// <summary>Server answer of Sales/Contract/Reconciliation.</summary>
    public sealed class ServiceReconciliationView
    {
        public Guid ContractId { get; set; }
        public bool Tariffed { get; set; }
        public bool RechargesUnavailable { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }
        public List<ServiceReconciliationRowView> Rows { get; set; } = new List<ServiceReconciliationRowView>();
    }
}
