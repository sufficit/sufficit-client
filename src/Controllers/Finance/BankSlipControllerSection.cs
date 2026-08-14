using Sufficit.Finance;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Finance
{
    /// <summary>
    /// Authenticated client for the canonical bank slip API.
    /// </summary>
    public sealed class BankSlipControllerSection : AuthenticatedControllerSection
    {
        private const string Prefix = "/finance/bankslip";
        private readonly JsonSerializerOptions _json;

        public BankSlipControllerSection(IAuthenticatedControllerBase controllerBase)
            : base(controllerBase)
        {
            _json = controllerBase.Json;
        }

        public async Task<BankSlipView?> RequestAsync(
            string idempotencyKey,
            BankSlipCreateRequest request,
            CancellationToken cancellationToken)
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, Prefix)
            {
                Content = JsonContent.Create(request, options: _json)
            };
            message.Headers.TryAddWithoutValidation("Idempotency-Key", idempotencyKey);

            using var response = await SendAsync(message, cancellationToken).ConfigureAwait(false);
            await response.EnsureSuccess(cancellationToken).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            try
            {
                return await response.Content
                    .ReadFromJsonAsync<BankSlipView>(_json, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (System.Text.Json.JsonException) when (TryGetBankSlipId(response.Headers.Location, out var bankSlipId))
            {
                // The POST is durable and idempotent. If a proxy or an older server returns
                // an unreadable 202 body, recover the accepted resource through Location
                // instead of presenting a false issuance failure to the operator.
                return await GetAsync(bankSlipId, cancellationToken).ConfigureAwait(false);
            }
            catch (NotSupportedException) when (TryGetBankSlipId(response.Headers.Location, out var bankSlipId))
            {
                return await GetAsync(bankSlipId, cancellationToken).ConfigureAwait(false);
            }
        }

        public Task<BankSlipView?> GetAsync(
            Guid bankSlipId,
            CancellationToken cancellationToken)
        {
            var uri = $"{Prefix}?bankSlipId={Uri.EscapeDataString(bankSlipId.ToString("D"))}";
            return Request<BankSlipView>(
                new HttpRequestMessage(HttpMethod.Get, uri),
                cancellationToken);
        }

        public Task<BankSlipPayerReadiness?> GetPayerReadinessAsync(
            Guid contextId,
            string? provider,
            CancellationToken cancellationToken)
        {
            var uri =
                $"{Prefix}/payerreadiness?contextId={Uri.EscapeDataString(contextId.ToString("D"))}";
            if (!string.IsNullOrWhiteSpace(provider))
            {
                uri += $"&provider={Uri.EscapeDataString(provider)}";
            }

            return Request<BankSlipPayerReadiness>(
                new HttpRequestMessage(HttpMethod.Get, uri),
                cancellationToken);
        }

        public Task<BankSlipSearchResult?> SearchAsync(
            BankSlipSearchParameters parameters,
            CancellationToken cancellationToken)
        {
            var uri = $"{Prefix}/search?{parameters.ToQueryString()}";
            return Request<BankSlipSearchResult>(
                new HttpRequestMessage(HttpMethod.Get, uri),
                cancellationToken);
        }

        public Task<BankSlipStatistics?> GetStatisticsAsync(
            BankSlipStatisticsParameters parameters,
            CancellationToken cancellationToken)
        {
            var uri = $"{Prefix}/statistics?{parameters.ToQueryString()}";
            return Request<BankSlipStatistics>(
                new HttpRequestMessage(HttpMethod.Get, uri),
                cancellationToken);
        }

        public Task<BankSlipProviderDiagnosticResult?> ExecuteDiagnosticAsync(
            BankSlipProviderDiagnosticParameters parameters,
            CancellationToken cancellationToken)
        {
            var uri = $"{Prefix}/diagnostics?{parameters.ToQueryString()}";
            return Request<BankSlipProviderDiagnosticResult>(
                new HttpRequestMessage(HttpMethod.Get, uri),
                cancellationToken);
        }

        public Task<List<BankSlipProviderNotificationView>?> GetProviderNotificationHistoryAsync(
            Guid? bankSlipId,
            string? provider,
            int limit,
            CancellationToken cancellationToken)
        {
            var uri = $"{Prefix}/providernotification?limit={Math.Min(200, Math.Max(1, limit))}";
            if (bankSlipId.HasValue && bankSlipId.Value != Guid.Empty)
            {
                uri += $"&bankSlipId={Uri.EscapeDataString(bankSlipId.Value.ToString("D"))}";
            }

            if (!string.IsNullOrWhiteSpace(provider))
            {
                uri += $"&provider={Uri.EscapeDataString(provider.Trim().ToLowerInvariant())}";
            }

            return Request<List<BankSlipProviderNotificationView>>(
                new HttpRequestMessage(HttpMethod.Get, uri),
                cancellationToken);
        }

        public Task<BankSlipSettingsView?> GetSettingsAsync(
            CancellationToken cancellationToken)
            => Request<BankSlipSettingsView>(
                new HttpRequestMessage(HttpMethod.Get, $"{Prefix}/settings"),
                cancellationToken);

        public Task<BankSlipSettingsView?> UpdateSettingsAsync(
            BankSlipSettingsUpdateRequest request,
            CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, $"{Prefix}/settings")
            {
                Content = JsonContent.Create(request, options: _json)
            };
            return Request<BankSlipSettingsView>(message, cancellationToken);
        }

        public Task SetPublicAccessAsync(
            Guid bankSlipId,
            bool enabled,
            CancellationToken cancellationToken)
        {
            var uri = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "{0}/publicaccess?bankSlipId={1:D}&enabled={2}",
                Prefix,
                bankSlipId,
                enabled.ToString().ToLowerInvariant());
            return Request(new HttpRequestMessage(HttpMethod.Post, uri), cancellationToken);
        }

        public Task<BankSlipView?> CancelAsync(
            Guid bankSlipId,
            CancellationToken cancellationToken)
        {
            var uri = $"{Prefix}/cancel?bankSlipId={Uri.EscapeDataString(bankSlipId.ToString("D"))}";
            return Request<BankSlipView>(
                new HttpRequestMessage(HttpMethod.Post, uri),
                cancellationToken);
        }

        public Task<BankSlipView?> RetryAsync(
            Guid bankSlipId,
            CancellationToken cancellationToken)
        {
            var uri = $"{Prefix}/retry?bankSlipId={Uri.EscapeDataString(bankSlipId.ToString("D"))}";
            return Request<BankSlipView>(
                new HttpRequestMessage(HttpMethod.Post, uri),
                cancellationToken);
        }

        private static bool TryGetBankSlipId(Uri? location, out Guid bankSlipId)
        {
            bankSlipId = Guid.Empty;
            var value = location?.ToString();
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var queryIndex = value.IndexOf('?');
            if (queryIndex < 0 || queryIndex == value.Length - 1)
                return false;

            var query = value.Substring(queryIndex + 1);
            foreach (var pair in query.Split(
                new[] { '&' },
                StringSplitOptions.RemoveEmptyEntries))
            {
                var separatorIndex = pair.IndexOf('=');
                if (separatorIndex <= 0)
                    continue;

                var key = Uri.UnescapeDataString(pair.Substring(0, separatorIndex));
                if (!string.Equals(key, "bankSlipId", StringComparison.OrdinalIgnoreCase))
                    continue;

                var rawId = Uri.UnescapeDataString(pair.Substring(separatorIndex + 1));
                return Guid.TryParse(rawId, out bankSlipId) && bankSlipId != Guid.Empty;
            }

            return false;
        }
    }
}
