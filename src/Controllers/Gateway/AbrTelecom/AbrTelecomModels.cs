using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sufficit.Client.Controllers.Gateway
{
    /// <summary>
    /// JSON-compatible mirror of <c>Sufficit.Gateway.AbrTelecom.AbrTelecomPortabilityRecord</c>.
    /// Kept separate so the client library does not pull Selenium.
    /// </summary>
    public sealed class AbrTelecomPortabilityRecord
    {
        [JsonPropertyName("date")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("providerName")]
        public string ProviderName { get; set; } = string.Empty;

        [JsonPropertyName("legalName")]
        public string LegalName { get; set; } = string.Empty;
    }

    /// <summary>
    /// JSON-compatible mirror of <c>Sufficit.Gateway.AbrTelecom.AbrTelecomQueryResult</c>.
    /// </summary>
    public sealed class AbrTelecomQueryResult
    {
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("records")]
        public IReadOnlyList<AbrTelecomPortabilityRecord> Records { get; set; } = new List<AbrTelecomPortabilityRecord>();

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}
