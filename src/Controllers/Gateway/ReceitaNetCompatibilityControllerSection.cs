using Microsoft.Extensions.Logging;
using Sufficit.Gateway.ReceitaNet;
using Sufficit.Net.Http;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Gateway
{
    /// <summary>
    /// Client section for ReceitaNet compatibility tests.
    /// </summary>
    public sealed class ReceitaNetCompatibilityControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = GatewayControllerSection.Controller;
        private const string Prefix = "/receitanetcompatibility";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public ReceitaNetCompatibilityControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        /// <summary>
        /// Executes a single ReceitaNet compatibility operation.
        /// </summary>
        public Task<ReceitaNetCompatibilityTestResult?> Test(ReceitaNetCompatibilityTestRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("receitanet compatibility test: {operation}", request.Operation);

            var uri = new Uri($"{Controller}{Prefix}/test", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request, null, _json)
            };

            return Request<ReceitaNetCompatibilityTestResult>(message, cancellationToken);
        }

        /// <summary>
        /// Executes a full ReceitaNet compatibility run.
        /// </summary>
        public Task<ReceitaNetCompatibilityTestReport?> TestAll(ReceitaNetCompatibilityTestRequest request, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/testall", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request, null, _json)
            };

            return Request<ReceitaNetCompatibilityTestReport>(message, cancellationToken);
        }

        /// <summary>
        /// Executes a lightweight validation for a single token.
        /// </summary>
        public Task<ReceitaNetTokenValidationResult?> ValidateToken(ReceitaNetCompatibilityTestRequest request, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/validatetoken", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request, null, _json)
            };

            return Request<ReceitaNetTokenValidationResult>(message, cancellationToken);
        }

        /// <summary>
        /// Retrieves persisted compatibility defaults for the selected gateway.
        /// </summary>
        public Task<ReceitaNetCompatibilityDefaults?> GetDefaults(ReceitaNetCompatibilityTestRequest request, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/defaults", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request, null, _json)
            };

            return Request<ReceitaNetCompatibilityDefaults>(message, cancellationToken);
        }

        protected override string[]? AnonymousPaths { get; } =
        {
            $"{Controller}{Prefix}/test",
            $"{Controller}{Prefix}/testall",
            $"{Controller}{Prefix}/validatetoken",
            $"{Controller}{Prefix}/defaults",
        };
    }
}