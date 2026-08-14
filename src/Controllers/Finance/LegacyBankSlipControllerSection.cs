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
    public sealed class LegacyBankSlipControllerSection : AuthenticatedControllerSection, ILegacyBankSlipController
    {
        private const string Controller = FinanceControllerSection.Controller;
        private const string Prefix = "/bankslip";

        private readonly JsonSerializerOptions _json;

        public LegacyBankSlipControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        [Authorize(Roles = $"{FinancialManagerRole.NormalizedName},{FinancialRole.NormalizedName},{ManagerRole.NormalizedName},{AdministratorRole.NormalizedName}")]
        public Task<IEnumerable<LegacyBankSlipInfo>> Search(LegacyBankSlipSearchParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/search", UriKind.Relative);
            var content = JsonContent.Create(parameters, null, _json);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content
            };
            return RequestMany<LegacyBankSlipInfo>(message, cancellationToken);
        }
    }
}
