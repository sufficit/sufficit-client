using Sufficit.Client.Controllers.Gateway;
using Sufficit.Net.Http;

namespace Sufficit.Client.Controllers
{
    public sealed class GatewayControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/gateway";

        public GatewayControllerSection(IAuthenticatedControllerBase cb) : base(cb) 
        {
            PhoneVox = new PhoneVoxControllerSection(cb);
            ReceitaFederal = new ReceitaFederalControllerSection(cb);
            ReceitaNet = new ReceitaNetControllerSection(cb);
            Wavoip = new WavoipControllerSection(cb);
        }

        public PhoneVoxControllerSection PhoneVox { get; }

        public ReceitaFederalControllerSection ReceitaFederal { get; }

        public ReceitaNetControllerSection ReceitaNet { get; }

        public WavoipControllerSection Wavoip { get; }
    }
}
