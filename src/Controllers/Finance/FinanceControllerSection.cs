using Sufficit.Net.Http;

namespace Sufficit.Client.Controllers
{
    public sealed class FinanceControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/finance";

        public FinanceControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            BankSlip = new BankSlipControllerSection(cb);
        }

        public BankSlipControllerSection BankSlip { get; }
    }
}
