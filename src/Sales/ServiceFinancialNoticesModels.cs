using System;

namespace Sufficit.Client.Sales
{
    /// <summary>One financial notice of a service, as computed by the server (label, detail, danger tone).</summary>
    public sealed class ServiceFinancialNoticeView
    {
        public string Label { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public bool Danger { get; set; }
    }

    /// <summary>Server answer of Sales/Contract/FinancialNotices for one primary service.</summary>
    public sealed class ServiceFinancialNoticesView
    {
        public Guid ContractId { get; set; }
        public ServiceFinancialNoticeView? Notice { get; set; }
        public ServiceFinancialNoticeView? RechargeNotice { get; set; }
        public string? ExtraCreditLine { get; set; }
        public bool Unavailable { get; set; }
    }
}
