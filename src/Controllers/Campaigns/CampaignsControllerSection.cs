using Sufficit.Campaigns.Models;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Campaigns
{
    /// <summary>Outreach campaigns API. Live events: <see cref="ICampaignRealtimeService"/>.</summary>
    public sealed class CampaignsControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/campaigns";

        private readonly JsonSerializerOptions _json;

        public CampaignsControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        public Task<IEnumerable<Campaign>> List(Guid? contextId, CampaignStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (contextId.HasValue) query["contextid"] = contextId.Value.ToString();
            if (status.HasValue) query["status"] = status.Value.ToString();
            return RequestMany<Campaign>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}?{query}", UriKind.Relative)), cancellationToken);
        }

        public Task<Campaign?> Get(Guid id, CancellationToken cancellationToken = default)
            => Request<Campaign>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}/{id:D}", UriKind.Relative)), cancellationToken);

        public Task<Campaign?> Create(Campaign campaign, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri(Controller, UriKind.Relative));
            message.Content = JsonContent.Create(campaign, null, _json);
            return Request<Campaign>(message, cancellationToken);
        }

        /// <summary>Editable fields only; context and status do not change here.</summary>
        public Task<Campaign?> Update(Campaign campaign, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, new Uri($"{Controller}/{campaign.Id:D}", UriKind.Relative));
            message.Content = JsonContent.Create(campaign, null, _json);
            return Request<Campaign>(message, cancellationToken);
        }

        /// <summary>Draft → Active ↔ Paused → Finished.</summary>
        public Task<Campaign?> SetStatus(Guid id, CampaignStatus status, CancellationToken cancellationToken = default)
            => Request<Campaign>(new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}/{id:D}/status?status={status}", UriKind.Relative)), cancellationToken);

        public Task<CampaignSummary?> GetSummary(Guid id, CancellationToken cancellationToken = default)
            => Request<CampaignSummary>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}/{id:D}/summary", UriKind.Relative)), cancellationToken);

        /// <summary>Adds targets; addresses already in the campaign are skipped. Returns how many were added.</summary>
        public Task<int?> AddTargets(Guid id, IEnumerable<CampaignTargetInput> targets, CancellationToken cancellationToken = default)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}/{id:D}/targets", UriKind.Relative));
            message.Content = JsonContent.Create(targets, null, _json);
            return RequestStruct<int>(message, cancellationToken);
        }

        public Task<IEnumerable<CampaignTarget>> ListTargets(Guid id, CampaignTargetStatus? status = null, int skip = 0, int take = 100, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (status.HasValue) query["status"] = status.Value.ToString();
            query["skip"] = skip.ToString();
            query["take"] = take.ToString();
            return RequestMany<CampaignTarget>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}/{id:D}/targets?{query}", UriKind.Relative)), cancellationToken);
        }

        public Task<IEnumerable<CampaignAttempt>> ListAttempts(Guid targetId, CancellationToken cancellationToken = default)
            => RequestMany<CampaignAttempt>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Controller}/targets/{targetId:D}/attempts", UriKind.Relative)), cancellationToken);

        /// <summary>Removes a target from the queue; it will not be contacted again.</summary>
        public Task CancelTarget(Guid targetId, CancellationToken cancellationToken = default)
            => Request(new HttpRequestMessage(HttpMethod.Delete, new Uri($"{Controller}/targets/{targetId:D}", UriKind.Relative)), cancellationToken);
    }
}
