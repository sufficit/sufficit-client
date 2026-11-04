using Microsoft.Extensions.Logging;
using Sufficit.Net.Http;
using Sufficit.Telephony.DIDs.Testing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    /// <summary>DID testing system: caller ids, parameters, starting tests and their history. Managers and administrators only.</summary>
    public sealed class TelephonyDIDTestControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/DIDTest";

        private readonly JsonSerializerOptions _json;

        public TelephonyDIDTestControllerSection(IAuthenticatedControllerBase cb) : base(cb) => _json = cb.Json;

        public async Task<IReadOnlyList<DIDTestOrigin>> GetOrigins(CancellationToken cancellationToken = default)
            => (await RequestMany<DIDTestOrigin>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/Origins", UriKind.Relative)), cancellationToken)).ToList();

        public Task<DIDTestOrigin?> SaveOrigin(DIDTestOrigin origin, CancellationToken cancellationToken = default)
            => Request<DIDTestOrigin>(Post("Origin", origin), cancellationToken);

        /// <summary>Null <paramref name="enabled"/> inverts the current state.</summary>
        public Task<DIDTestOrigin?> ToggleOrigin(Guid id, bool? enabled, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            if (enabled.HasValue) query["enabled"] = enabled.Value ? "true" : "false";
            return Request<DIDTestOrigin>(new HttpRequestMessage(new HttpMethod("PATCH"), new Uri($"{Controller}{Prefix}/Origin/Toggle?{query}", UriKind.Relative)), cancellationToken);
        }

        public Task DeleteOrigin(Guid id, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            return Request(new HttpRequestMessage(HttpMethod.Delete, new Uri($"{Controller}{Prefix}/Origin?{query}", UriKind.Relative)), cancellationToken);
        }

        public Task<DIDTestSettings?> GetSettings(CancellationToken cancellationToken = default)
            => Request<DIDTestSettings>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/Settings", UriKind.Relative)), cancellationToken);

        public Task<DIDTestSettings?> SaveSettings(DIDTestSettings settings, CancellationToken cancellationToken = default)
            => Request<DIDTestSettings>(Post("Settings", settings), cancellationToken);

        /// <summary>Starts tests for one number or a pasted list; numbers the API cannot accept come back with the reason.</summary>
        public Task<DIDTestStartResult?> Start(DIDTestStartRequest request, CancellationToken cancellationToken = default)
            => Request<DIDTestStartResult>(Post("Start", request), cancellationToken);

        public async Task<IReadOnlyList<DIDTestRun>> GetRuns(DIDTestRunSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrWhiteSpace(parameters.Did)) query["did"] = parameters.Did;
            if (parameters.Status.HasValue) query["status"] = parameters.Status.Value.ToString();
            if (parameters.BatchId.HasValue) query["batchid"] = parameters.BatchId.Value.ToString();
            if (parameters.Start.HasValue) query["start"] = parameters.Start.Value.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);
            if (parameters.End.HasValue) query["end"] = parameters.End.Value.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);
            query["limit"] = parameters.Limit.ToString(CultureInfo.InvariantCulture);
            return (await RequestMany<DIDTestRun>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}{Prefix}/Runs?{query}", UriKind.Relative)), cancellationToken)).ToList();
        }

        /// <summary>Cancels waiting tests, one or a whole batch. Returns how many were cancelled.</summary>
        public async Task<int> Cancel(Guid? id, Guid? batchId, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (id.HasValue) query["id"] = id.Value.ToString();
            if (batchId.HasValue) query["batchid"] = batchId.Value.ToString();
            return await RequestStruct<int>(new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/Cancel?{query}", UriKind.Relative)), cancellationToken) ?? 0;
        }

        private HttpRequestMessage Post<T>(string action, T body) => new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}{Prefix}/{action}", UriKind.Relative))
        {
            Content = JsonContent.Create(body, null, _json),
        };
    }
}
