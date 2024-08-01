using Sufficit.Client.Controllers.Gateway;
using Sufficit.Client.Controllers.Telephony;

namespace Sufficit.Client.Controllers
{
    public sealed class GatewayControllerSection : ControllerSection
    {
        public const string Controller = "/gateway";

        public GatewayControllerSection(APIClientService service) : base(service) 
        {
            PhoneVox = new PhoneVoxControllerSection(service);
            ReceitaNet = new ReceitaNetControllerSection(service);
        }

        public PhoneVoxControllerSection PhoneVox { get; }

        public ReceitaNetControllerSection ReceitaNet { get; }
    }
}
