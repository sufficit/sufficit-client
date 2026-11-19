using Sufficit.Net.Http;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    /// <summary>Native queue management in EndPoints. Dispose the returned HTTP response after reading it.</summary>
    public sealed class TelephonyQueueControllerSection : AuthenticatedControllerSection
    {
        public TelephonyQueueControllerSection(IAuthenticatedControllerBase controller) : base(controller)
        {
            IsConfigured = controller.Client.BaseAddress != null;
        }

        public bool IsConfigured { get; }

        public Task<HttpResponseMessage> GetNodes(Guid context, CancellationToken token = default)
            => Send(HttpMethod.Get, Scope(context) + "/nodes", token);

        public Task<HttpResponseMessage> GetQueues(Guid context, Guid node = default, CancellationToken token = default)
            => Send(HttpMethod.Get, Scope(context, node) + "/queues", token);

        public Task<HttpResponseMessage> GetSnapshot(Guid context, Guid node, CancellationToken token = default)
            => Send(HttpMethod.Get, Node(context, node) + "/snapshot", token);

        public Task<HttpResponseMessage> GetIngress(Guid context, Guid node, Guid queue, CancellationToken token = default)
            => Send(HttpMethod.Get, Node(context, node) + $"/queues/{Required(queue):N}/ingress", token);

        public Task<HttpResponseMessage> GetMembers(Guid context, Guid node, Guid queue, CancellationToken token = default)
            => Send(HttpMethod.Get, Members(context, node, queue), token);

        public Task<HttpResponseMessage> SearchAgents(Guid context, Guid node, string search, bool allContexts, CancellationToken token = default)
            => Send(HttpMethod.Get, Scope(context, node) + "/agent-destinations?search=" + Uri.EscapeDataString(search) +
                "&allContexts=" + (allContexts ? "true" : "false"), token);

        /// <summary>Provide the current strong ETag to update. A null revision requests conditional creation.</summary>
        public Task<HttpResponseMessage> SaveQueue(Guid context, Guid node, Guid queue, HttpContent edit,
            string? revision, CancellationToken token = default)
            => Send(HttpMethod.Put, Scope(context, node) + $"/queues/{Required(queue):N}", token, edit, revision, conditional: true);

        public Task<HttpResponseMessage> SaveMember(Guid context, Guid node, Guid queue, Guid member, HttpContent edit,
            string? revision, CancellationToken token = default)
            => Send(HttpMethod.Put, Members(context, node, queue) + $"/{Required(member):N}", token, edit, revision, conditional: true);

        public Task<HttpResponseMessage> DeleteMember(Guid context, Guid node, Guid queue, Guid member,
            string revision, CancellationToken token = default)
            => Send(HttpMethod.Delete, Members(context, node, queue) + $"/{Required(member):N}", token, revision: revision, conditional: true);

        public Task<HttpResponseMessage> ReorderMembers(Guid context, Guid node, Guid queue, HttpContent order, CancellationToken token = default)
            => Send(HttpMethod.Put, Scope(context, node) + $"/queues/{Required(queue):N}/member-order", token, order);

        private static string Scope(Guid context, Guid node = default)
            => $"api/queue/contexts/{Required(context):N}" + (node == Guid.Empty ? "" : $"/nodes/{node:N}");
        private static string Node(Guid context, Guid node) => Scope(context, Required(node));
        private static string Members(Guid context, Guid node, Guid queue) => Scope(context, node) + $"/queues/{Required(queue):N}/members";
        private static Guid Required(Guid id) => id != Guid.Empty ? id : throw new ArgumentException("A queue resource identifier is required.");

        private async Task<HttpResponseMessage> Send(HttpMethod method, string path, CancellationToken token,
            HttpContent? content = null, string? revision = null, bool conditional = false)
        {
            using var request = new HttpRequestMessage(method, new Uri(path, UriKind.Relative)) { Content = content };
            if (conditional)
            {
                if (revision is null && method == HttpMethod.Put) request.Headers.IfNoneMatch.Add(EntityTagHeaderValue.Any);
                else if (EntityTagHeaderValue.TryParse(revision, out var tag) && tag != null && !tag.IsWeak &&
                    tag.Tag != "*")
                    request.Headers.IfMatch.Add(tag);
                else throw new ArgumentException("A strong queue revision is required.", nameof(revision));
            }
            return await SendAsync(request, token);
        }
    }
}
