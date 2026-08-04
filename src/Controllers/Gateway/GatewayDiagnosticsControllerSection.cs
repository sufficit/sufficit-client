using Sufficit.Gateway;
using Sufficit.Net.Http;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Gateway
{
    /// <summary>
    /// Client for the controlled, provider-wide gateway laboratory.
    /// </summary>
    public sealed class GatewayDiagnosticsControllerSection : AuthenticatedControllerSection
    {
        private const string Prefix = "/v2/Gateway/Diagnostics";

        public GatewayDiagnosticsControllerSection(IAuthenticatedControllerBase cb)
            : base(cb)
        {
        }

        public Task<GatewayDiagnosticCatalog?> GetCatalogAsync(
            string provider,
            CancellationToken cancellationToken)
        {
            var uri = $"{Prefix}/catalog?provider={Uri.EscapeDataString(provider ?? string.Empty)}";
            return Request<GatewayDiagnosticCatalog>(
                new HttpRequestMessage(HttpMethod.Get, uri),
                cancellationToken);
        }

        public Task<GatewayDiagnosticResult?> ExecuteAsync(
            GatewayDiagnosticRequest request,
            CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, $"{Prefix}/execute")
            {
                Content = JsonContent.Create(request)
            };
            return Request<GatewayDiagnosticResult>(message, cancellationToken);
        }
    }
}
