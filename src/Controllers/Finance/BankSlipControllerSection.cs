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
    public sealed class BankSlipControllerSection : AuthenticatedControllerSection, IBankSlipController
    {
        private const string Controller = FinanceControllerSection.Controller;
        private const string Prefix = "/bankslip";

        private readonly JsonSerializerOptions _json;

        public BankSlipControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        ////// <inheritdoc cref="IMessagesController.GetDetails(MessageDetailsSearchParameters, CancellationToken) "/>
        [Authorize(Roles = $"{FinancialManagerRole.NormalizedName},{FinancialRole.NormalizedName},{ManagerRole.NormalizedName},{AdministratorRole.NormalizedName}")]
        public Task<IEnumerable<BankSlipInfo>> Search(BankSlipSearchParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/search", UriKind.Relative);
            var content = JsonContent.Create(parameters, null, _json);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content
            };
            return RequestMany<BankSlipInfo>(message, cancellationToken);
        }
    }
}
