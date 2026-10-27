using Microsoft.AspNetCore.Authorization;
using Sufficit.Cadences;
using Sufficit.Identity;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    /// <summary>Cadences ("réguas") shared by every module: notification, cut-off and control schedules.</summary>
    public sealed class CadencesControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = "/cadences";
        private const string Roles = $"{Sufficit.Sales.SalesManagerRole.NormalizedName},{ManagerRole.NormalizedName},{AdministratorRole.NormalizedName}";
        private readonly System.Text.Json.JsonSerializerOptions _json;

        public CadencesControllerSection(IAuthenticatedControllerBase cb) : base(cb) { _json = cb.Json; }

        [Authorize(Roles = Roles)]
        public Task<IEnumerable<Cadence>> Search(CadenceSearchParameters parameters, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}/search", UriKind.Relative));
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<Cadence>(message, cancellationToken);
        }

        [Authorize(Roles = Roles)]
        public Task<Cadence?> Get(Guid id, CancellationToken cancellationToken)
            => Request<Cadence>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}?id={id:D}", UriKind.Relative)), cancellationToken);

        /// <summary>Active cadence of a module and kind for the first candidate target that has one (most specific first).</summary>
        [Authorize(Roles = Roles)]
        public Task<Cadence?> Resolve(CadenceResolveRequest request, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}/resolve", UriKind.Relative));
            message.Content = JsonContent.Create(request, null, _json);
            return Request<Cadence>(message, cancellationToken);
        }

        [Authorize(Roles = Roles)]
        public Task<IEnumerable<CadenceRevision>> Revisions(Guid id, CancellationToken cancellationToken)
            => RequestMany<CadenceRevision>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}/revisions?id={id:D}", UriKind.Relative)), cancellationToken);

        /// <summary>Saves a new version. A conflict (stale version, second active cadence, invalid steps) carries a user facing message.</summary>
        [Authorize(Roles = Roles)]
        public Task<Cadence?> Save(Cadence cadence, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri(Controller, UriKind.Relative));
            message.Content = JsonContent.Create(cadence, null, _json);
            return Request<Cadence>(message, cancellationToken);
        }

        [Authorize(Roles = Roles)]
        public Task Delete(Guid id, CancellationToken cancellationToken)
            => Request(new HttpRequestMessage(HttpMethod.Delete, new Uri($"{Controller}?id={id:D}", UriKind.Relative)), cancellationToken);
    }
}
