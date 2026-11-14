using Microsoft.AspNetCore.Authorization;
using Sufficit.Finance;
using Sufficit.Identity;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Finance
{
    /// <summary>
    ///     Company accounts payable (Finance/Payables): supplier bills and commission withdrawal invoices,
    ///     with payments and attachments (storage objects whose context is the account id). Internal managers only.
    /// </summary>
    [Authorize(Roles = $"{AdministratorRole.NormalizedName},{ManagerRole.NormalizedName}")]
    public sealed class PayablesControllerSection : AuthenticatedControllerSection
    {
        private const string Prefix = FinanceControllerSection.Controller + "/payables";
        private readonly JsonSerializerOptions _json;

        public PayablesControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        public Task<IEnumerable<Payable>> Search(PayableSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (parameters?.ContextId is { } context) query["contextId"] = context.ToString("D");
            if (parameters?.Status is { } status) query["status"] = status.ToString();
            if (parameters?.BeneficiaryId is { } beneficiary) query["beneficiaryId"] = beneficiary.ToString("D");
            if (!string.IsNullOrWhiteSpace(parameters?.Text)) query["text"] = parameters!.Text;
            return RequestMany<Payable>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Prefix}?{query}", UriKind.Relative)), cancellationToken);
        }

        public Task<Payable?> ById(Guid id, CancellationToken cancellationToken = default)
            => Request<Payable>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Prefix}/byid?id={id:D}", UriKind.Relative)), cancellationToken);

        /// <summary>Creates (empty id) or changes an account; changes need the revision read.</summary>
        public Task<Payable?> Save(PayableSaveRequest request, CancellationToken cancellationToken = default)
            => Request<Payable>(new HttpRequestMessage(HttpMethod.Put, new Uri(Prefix, UriKind.Relative))
                { Content = JsonContent.Create(request, null, _json) }, cancellationToken);

        /// <summary>Records money paid on the account.</summary>
        public Task<Payable?> Pay(PayablePaymentRequest request, CancellationToken cancellationToken = default)
            => Request<Payable>(new HttpRequestMessage(HttpMethod.Post, new Uri($"{Prefix}/payments", UriKind.Relative))
                { Content = JsonContent.Create(request, null, _json) }, cancellationToken);
    }
}
