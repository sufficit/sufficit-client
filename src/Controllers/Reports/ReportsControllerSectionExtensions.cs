using Sufficit.Reports;
using Sufficit.Reports.Models;
using Sufficit.Telephony.Reports;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Sufficit.Reports.Models.Ids;
using static Sufficit.Telephony.Reports.ModelsIds;

namespace Sufficit.Client.Controllers.Reports
{
    public static class ReportsControllerSectionExtensions
    {       
        public static Task<ReportBaseGeneric<BilledCallsByDIDReportItem>> BilledCallsByDIDReport (this ReportsControllerSection source, ReportRequestParameters parameters, CancellationToken cancellationToken = default)
        {
            parameters.ModelId = Guid.Parse(BilledCallsByDIDId);
            return source.GetReport<BilledCallsByDIDReportItem>(parameters, cancellationToken)!;
        }

        public static Task<ReportBaseGeneric<ComplianceReportItem>> ComplianceReport (this ReportsControllerSection source, ReportRequestParameters parameters, CancellationToken cancellationToken = default)
        {
            parameters.ModelId = Guid.Parse(ComplianceReportId);
            return source.GetReport<ComplianceReportItem>(parameters, cancellationToken)!;
        }
    }
}
