using Sufficit.Net.Http;

namespace Sufficit.Client.Controllers.Gateway
{
    public sealed class GatewayControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/Gateway";

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