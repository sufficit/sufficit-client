using Microsoft.AspNetCore.Authorization;
using Sufficit.Exchange;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Exchange
{
    public sealed class ExchangeReadsControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = ExchangeControllerSection.Controller;
        private const string Prefix = "/messages/reads";

        private readonly JsonSerializerOptions _json;

        public ExchangeReadsControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        /// <summary>
        /// Get read receipts by reference ID
        /// </summary>
        [Authorize(Roles = $"{Sufficit.Identity.ManagerRole.NormalizedName},{Sufficit.Identity.AdministratorRole.NormalizedName}")]
        public async IAsyncEnumerable<IReadReceipt> GetByReferenceId(Guid id, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/ByReferenceId/{id}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);

            // Since this returns IAsyncEnumerable, we need to handle streaming response
            var response = await SendAsync(message, cancellationToken);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await foreach (var item in JsonSerializer.DeserializeAsyncEnumerable<IReadReceipt>(stream, _json, cancellationToken))
            {
                if (item != null)
                    yield return item;
            }
        }
    }
}
