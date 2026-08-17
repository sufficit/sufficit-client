using Sufficit.Net.Http;

namespace Sufficit.Client.Controllers.Gateway
{
    public sealed class GatewayControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/Gateway";

        public GatewayControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            AsaasInvoices = new AsaasInvoicesControllerSection(cb);
            Diagnostics = new GatewayDiagnosticsControllerSection(cb);
            FluxTelecomSms = new FluxTelecomSmsControllerSection(cb);
            PhoneVox = new PhoneVoxControllerSection(cb);
            ReceitaFederal = new ReceitaFederalControllerSection(cb);
            ReceitaNet = new ReceitaNetControllerSection(cb);
            Wavoip = new WavoipControllerSection(cb);
            WhatsApp = new WhatsAppControllerSection(cb);
            Zabbix = new ZabbixControllerSection(cb);
            AbrTelecom = new AbrTelecomControllerSection(cb);
        }

        public AsaasInvoicesControllerSection AsaasInvoices { get; }

        public GatewayDiagnosticsControllerSection Diagnostics { get; }

        public FluxTelecomSmsControllerSection FluxTelecomSms { get; }

        public AbrTelecomControllerSection AbrTelecom { get; }

        public PhoneVoxControllerSection PhoneVox { get; }

        public ReceitaFederalControllerSection ReceitaFederal { get; }

        public ReceitaNetControllerSection ReceitaNet { get; }

        public WavoipControllerSection Wavoip { get; }

        public WhatsAppControllerSection WhatsApp { get; }

        public ZabbixControllerSection Zabbix { get; }
    }
}
