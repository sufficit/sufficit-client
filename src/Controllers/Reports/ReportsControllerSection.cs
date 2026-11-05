using Sufficit.Net.Http;
using Sufficit.Reports;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Reports
{
    public sealed class ReportsControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/reports";
        private readonly JsonSerializerOptions _json;

        public ReportsControllerSection(IAuthenticatedControllerBase cb) : base(cb) 
        {
            _json = cb.Json;
        }

        /// <summary>
        ///     Gets a generic report, must be used with a model id
        /// </summary>
        public Task<ReportBaseGeneric<TItems>> GetReport<TItems>(ReportRequestParameters parameters, CancellationToken cancellationToken = default)
        {
            if (parameters.ModelId == Guid.Empty)
                throw new ArgumentException("ModelId must be set", nameof(parameters));

            var uri = new Uri(Controller, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return Request<ReportBaseGeneric<TItems>>(message, cancellationToken)!;
        }

        public Task<ReportProgress?> GetProgress(Guid id, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/progress";

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<ReportProgress>(message, cancellationToken)!;
        }

        /// <summary>Discover customer-scoped telephony report capabilities.</summary>
        public Task<System.Collections.Generic.List<Sufficit.Telephony.Reports.TelephonyReportModel>> TelephonyCatalog(CancellationToken ct = default)
            => Request<System.Collections.Generic.List<Sufficit.Telephony.Reports.TelephonyReportModel>>(
                new HttpRequestMessage(HttpMethod.Get, "/Telephony/Reports/Catalog"), ct)!;

        /// <summary>Complete snapshot; cancellation ends the HTTP request. Excel is optional on the same snapshot.</summary>
        public Task<Sufficit.Telephony.Reports.TelephonyReportResult> TelephonyReport(string model, Sufficit.Telephony.Reports.TelephonyReportRequest parameters, CancellationToken ct = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "/Telephony/Reports/" + Uri.EscapeDataString(model));
            message.Content = JsonContent.Create(parameters, null, _json);
            return Request<Sufficit.Telephony.Reports.TelephonyReportResult>(message, ct)!;
        }

        protected override string[]? AnonymousPaths { get; } = { $"{Controller}/progress" };
    }
}
