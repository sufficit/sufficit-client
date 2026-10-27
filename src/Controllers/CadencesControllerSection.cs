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
    /// <summary>Cadences ("réguas"): a clock of milestones contributed by the modules; cases and occurrences.</summary>
    public sealed class CadencesControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = "/cadences";
        private const string Roles = $"{Sufficit.Sales.SalesManagerRole.NormalizedName},{ManagerRole.NormalizedName},{AdministratorRole.NormalizedName}";
        private readonly System.Text.Json.JsonSerializerOptions _json;

        public CadencesControllerSection(IAuthenticatedControllerBase cb) : base(cb) { _json = cb.Json; }

        /// <summary>Milestones contributed by the modules and whether cadences run in simulation.</summary>
        [Authorize(Roles = Roles)]
        public Task<CadenceCatalog?> Milestones(CancellationToken cancellationToken)
            => Request<CadenceCatalog>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}/milestones", UriKind.Relative)), cancellationToken);

        [Authorize(Roles = Roles)]
        public Task<IEnumerable<Cadence>> Search(CadenceSearchParameters parameters, CancellationToken cancellationToken)
            => RequestMany<Cadence>(Post($"{Controller}/search", parameters), cancellationToken);

        [Authorize(Roles = Roles)]
        public Task<Cadence?> Get(Guid id, CancellationToken cancellationToken)
            => Request<Cadence>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}?id={id:D}", UriKind.Relative)), cancellationToken);

        [Authorize(Roles = Roles)]
        public Task<IEnumerable<CadenceRevision>> Revisions(Guid id, CancellationToken cancellationToken)
            => RequestMany<CadenceRevision>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}/revisions?id={id:D}", UriKind.Relative)), cancellationToken);

        /// <summary>Saves a new version. A conflict (stale version, unknown milestone) carries a user facing message.</summary>
        [Authorize(Roles = Roles)]
        public Task<Cadence?> Save(Cadence cadence, CancellationToken cancellationToken)
            => Request<Cadence>(Post(Controller, cadence), cancellationToken);

        [Authorize(Roles = Roles)]
        public Task Delete(Guid id, CancellationToken cancellationToken)
            => Request(new HttpRequestMessage(HttpMethod.Delete, new Uri($"{Controller}?id={id:D}", UriKind.Relative)), cancellationToken);

        [Authorize(Roles = Roles)]
        public Task<IEnumerable<CadenceCase>> SearchCases(CadenceCaseSearchParameters parameters, CancellationToken cancellationToken)
            => RequestMany<CadenceCase>(Post($"{Controller}/cases/search", parameters), cancellationToken);

        [Authorize(Roles = Roles)]
        public Task<IEnumerable<CadenceOccurrence>> Occurrences(Guid caseId, CancellationToken cancellationToken)
            => RequestMany<CadenceOccurrence>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}/cases/occurrences?id={caseId:D}", UriKind.Relative)), cancellationToken);

        /// <summary>Opened by the module that owns the subject. Idempotent while the case is open.</summary>
        [Authorize(Roles = Roles)]
        public Task<CadenceCase?> OpenCase(CadenceOpenCaseRequest request, CancellationToken cancellationToken)
            => Request<CadenceCase>(Post($"{Controller}/cases/open", request), cancellationToken);

        [Authorize(Roles = Roles)]
        public Task<CadenceCase?> CloseCase(CadenceCloseCaseRequest request, CancellationToken cancellationToken)
            => Request<CadenceCase>(Post($"{Controller}/cases/close", request), cancellationToken);

        [Authorize(Roles = Roles)]
        public Task<CadenceCase?> MoveAnchor(CadenceMoveAnchorRequest request, CancellationToken cancellationToken)
            => Request<CadenceCase>(Post($"{Controller}/cases/move", request), cancellationToken);

        /// <summary>Daily tick (administrators): records and publishes milestones due up to <paramref name="date"/>.</summary>
        [Authorize(Roles = AdministratorRole.NormalizedName)]
        public Task<CadenceRunResult?> Run(DateTime? date, CancellationToken cancellationToken)
        {
            var query = date.HasValue ? $"?date={date.Value:yyyy-MM-dd}" : string.Empty;
            return Request<CadenceRunResult>(new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}/run{query}", UriKind.Relative)), cancellationToken);
        }

        private HttpRequestMessage Post<T>(string uri, T body)
            => new(HttpMethod.Post, new Uri(uri, UriKind.Relative)) { Content = JsonContent.Create(body, null, _json) };
    }
}
