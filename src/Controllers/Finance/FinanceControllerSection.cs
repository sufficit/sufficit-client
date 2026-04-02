using Sufficit;
using Sufficit.Contacts;
using Sufficit.Net.Http;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Finance
{
    public sealed class FinanceControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/finance";

        public FinanceControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            BankSlip = new BankSlipControllerSection(cb);
            ElectronicInvoice = new ElectronicInvoiceControllerSection(cb);
        }

        public BankSlipControllerSection BankSlip { get; }

        public ElectronicInvoiceControllerSection ElectronicInvoice { get; }

        public Task<IdTitlePair?> GetEntity(Guid id, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}/entity?id={id:N}", UriKind.Relative));
            return Request<IdTitlePair>(message, cancellationToken);
        }

        public Task<IdTitlePair?> GetEntity(string document, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(document)] = document;
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}/entity?{query}", UriKind.Relative));
            return Request<IdTitlePair>(message, cancellationToken);
        }
    }
}
