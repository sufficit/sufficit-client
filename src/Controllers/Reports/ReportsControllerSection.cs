using Sufficit.Net.Http;
using Sufficit.Reports;
using Sufficit.Telephony.Reports;
using System.Net;
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

        private const string Jobs = "/Telephony/Reports/Jobs";

        public Task<TelephonyReportJob> StartTelephonyReport(string model, TelephonyReportRequest request, Guid? replaceCompleted, CancellationToken ct)
        {
            var url = $"{Jobs}/{Uri.EscapeDataString(model)}";
            if (replaceCompleted.HasValue) url += $"?replaceCompleted={replaceCompleted:D}";
            return SendJob(HttpMethod.Post, url, request, ct);
        }

        public async Task<TelephonyReportJob?> FindTelephonyReport(string model, TelephonyReportRequest request, CancellationToken ct)
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, $"{Jobs}/{Uri.EscapeDataString(model)}/lookup")
            {
                Content = JsonContent.Create(request, options: _json)
            };
            using var response = await SendAsync(message, ct).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            await response.EnsureSuccess(ct).ConfigureAwait(false);
            return await response.Content.ReadFromJsonAsync<TelephonyReportJob>(_json, ct).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Report recovery returned an empty response.");
        }

        public Task<TelephonyReportJob> TelephonyReportStatus(Guid context, Guid id, CancellationToken ct)
            => SendJob(HttpMethod.Get, $"{Jobs}/{id:D}?contextId={context:D}", null, ct);

        public Task<TelephonyReportJob> CancelTelephonyReport(Guid context, Guid id, CancellationToken ct)
            => SendJob(HttpMethod.Delete, $"{Jobs}/{id:D}?contextId={context:D}", null, ct);

        public async Task<TelephonyReportResult> TelephonyReportResult(Guid context, Guid id, CancellationToken ct)
        {
            using var message = new HttpRequestMessage(HttpMethod.Get, $"{Jobs}/{id:D}/result?contextId={context:D}");
            return await Request<TelephonyReportResult>(message, ct).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Report result returned an empty response.");
        }

        private async Task<TelephonyReportJob> SendJob(HttpMethod method, string url, TelephonyReportRequest? request, CancellationToken ct)
        {
            using var message = new HttpRequestMessage(method, url);
            if (request != null) message.Content = JsonContent.Create(request, options: _json);
            return await Request<TelephonyReportJob>(message, ct).ConfigureAwait(false)
                ?? throw new InvalidOperationException("Report job returned an empty response.");
        }

        protected override string[]? AnonymousPaths { get; } = { $"{Controller}/progress" };
    }
}
