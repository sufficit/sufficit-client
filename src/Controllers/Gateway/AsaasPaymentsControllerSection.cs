using Sufficit.Finance;
using Sufficit.Identity;
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
    /// Private machine-to-machine Asaas checkout payments surface. Provider
    /// credentials, environment and beneficiary pinning live on the API host;
    /// this section authenticates with the checkout integration key resolved
    /// from the token provider registered by the consuming application. The
    /// wire contracts live in Sufficit.Base (Sufficit.Finance).
    /// </summary>
    public sealed class AsaasPaymentsControllerSection : AuthenticatedControllerSection
    {
        public const string HeaderName = "X-Checkout-Payments-Key";
        private const string Prefix = "/Gateway/Asaas";

        private readonly ITokenProvider _integrationKey;
        private readonly JsonSerializerOptions _json;

        public AsaasPaymentsControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _integrationKey = cb.Tokens;
            _json = cb.Json;
        }

        /// <summary>
        /// Replaces the bearer flow with the checkout integration key header:
        /// this section is consumed by anonymous hosted applications.
        /// </summary>
        protected override async ValueTask Authenticate(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Contains(HeaderName))
                return;

            var key = await _integrationKey.GetTokenAsync().ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(key))
                throw new UnauthenticatedExpection("checkout payments integration key not available at this time");

            request.Headers.TryAddWithoutValidation(HeaderName, key.Trim());
        }

        /// <summary>Returns billing details to the trusted checkout server only.</summary>
        public Task<CheckoutPaymentPayerRequest?> GetRegisteredPayerAsync(
            Guid customerId,
            CancellationToken cancellationToken)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("A customer identifier is required.", nameof(customerId));
            return Request<CheckoutPaymentPayerRequest>(
                new HttpRequestMessage(HttpMethod.Get, Prefix + "/CheckoutPayers/" + customerId.ToString("D")),
                cancellationToken);
        }

        /// <summary>
        /// Creates a Pix, bank slip or hosted card charge for a checkout session.
        /// Bank slip results include optional Pix presentation; card results
        /// expose the validated Asaas invoice URL.
        /// </summary>
        public Task<CheckoutPaymentView?> CreatePaymentAsync(
            CheckoutPaymentCreateRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));
            var message = new HttpRequestMessage(HttpMethod.Post, Prefix + "/Payments")
            {
                Content = JsonContent.Create(request, options: _json)
            };
            return Request<CheckoutPaymentView>(message, cancellationToken);
        }

        /// <summary>Pays an existing card charge; request data is transient and must never be logged.</summary>
        public Task<CheckoutCardPayView?> PayCardAsync(
            CheckoutCardPayRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));
            var message = new HttpRequestMessage(HttpMethod.Post, Prefix + "/Payments/Card/Pay")
            {
                Content = JsonContent.Create(request, options: _json)
            };
            return Request<CheckoutCardPayView>(message, cancellationToken);
        }

        /// <summary>
        /// Gets the current provider state of a charge with its Pix
        /// presentation. Returns null when the charge is unknown.
        /// </summary>
        public Task<CheckoutPaymentView?> GetPaymentAsync(
            string providerChargeId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(providerChargeId))
                throw new ArgumentException("A provider charge identifier is required.", nameof(providerChargeId));

            var uri = Prefix + "/Payments/" + Uri.EscapeDataString(providerChargeId.Trim());
            return Request<CheckoutPaymentView>(
                new HttpRequestMessage(HttpMethod.Get, uri),
                cancellationToken);
        }

        /// <summary>
        /// Verifies that the configured Asaas credential belongs to the
        /// expected Sufficit beneficiary before taking payments.
        /// </summary>
        public Task<CheckoutAccountView?> VerifyAccountAsync(CancellationToken cancellationToken)
            => Request<CheckoutAccountView>(
                new HttpRequestMessage(HttpMethod.Post, Prefix + "/Account/Verify"),
                cancellationToken);

        /// <summary>
        /// Ensures the Asaas webhook subscription delivering payment events to
        /// the public checkout application exists and matches the request.
        /// </summary>
        public Task<CheckoutWebhookProvisioningView?> ProvisionWebhookAsync(
            CheckoutWebhookProvisioningRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));
            var message = new HttpRequestMessage(HttpMethod.Post, Prefix + "/Webhooks/Provisioning")
            {
                Content = JsonContent.Create(request, options: _json)
            };
            return Request<CheckoutWebhookProvisioningView>(message, cancellationToken);
        }

        /// <summary>
        /// Validates the provider signature of a webhook event received by the
        /// public checkout application and returns the parsed notification.
        /// </summary>
        public Task<CheckoutWebhookVerificationView?> VerifyWebhookEventAsync(
            CheckoutWebhookVerifyRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));
            var message = new HttpRequestMessage(HttpMethod.Post, Prefix + "/Webhooks/Events/Verify")
            {
                Content = JsonContent.Create(request, options: _json)
            };
            return Request<CheckoutWebhookVerificationView>(message, cancellationToken);
        }
    }
}
