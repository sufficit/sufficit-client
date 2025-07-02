using Sufficit.Net.Http;

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
    }
}
