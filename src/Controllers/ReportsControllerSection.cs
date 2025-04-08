using Sufficit.Identity;
using Sufficit.Net.Http;
using Sufficit.Reports;
using Sufficit.Telephony.Reports;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class ReportsControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/reports";

        public ReportsControllerSection(IAuthenticatedControllerBase cb) : base(cb) { }   

        public Task<BilledCallsByDIDReport> BilledCallsByDIDReport (ReportParametersNew parameters, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}";
            var query = parameters.ToQueryString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);

            return Request<BilledCallsByDIDReport>(message, cancellationToken)!;
        }

        public Task<ReportStatus?> GetStatus(Guid id, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/status";

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<ReportStatus>(message, cancellationToken)!;
        }

        protected override string[]? AnonymousPaths { get; } = { $"{Controller}/status" };
    }
}
