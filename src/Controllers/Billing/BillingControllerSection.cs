using Sufficit.Net.Http;

namespace Sufficit.Client.Controllers.Billing
{
    public sealed class BillingControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/billing";

        public BillingControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            ElectronicInvoice = new ElectronicInvoiceControllerSection(cb);
        }

        public ElectronicInvoiceControllerSection ElectronicInvoice { get; }
    }
}
