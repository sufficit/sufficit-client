using Sufficit.Net.Http;

namespace Sufficit.Client.Controllers.Billing
{
    public sealed class BillingControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/billing";

        public BillingControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            EletronicInvoice = new EletronicInvoiceControllerSection(cb);
        }

        public EletronicInvoiceControllerSection EletronicInvoice { get; }
    }
}
