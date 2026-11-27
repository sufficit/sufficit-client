using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed partial class TelephonyCallControllerSection
    {
        private static string ChannelTagsPath(Guid contextId, string? linkedId = null)
            => $"Telephony/Call/ChannelTags?contextid={contextId:D}"
                + (linkedId == null ? string.Empty : $"&linkedid={Uri.EscapeDataString(linkedId)}");

        /// <summary>Returns the channel's tag assignments, including expired ones. Null means the call or route is unavailable.</summary>
        public Task<CallChannelTag[]?> ReadChannelTags(Guid contextId, string linkedId, CancellationToken ct)
            => ReadTags<CallChannelTag[]>(HttpMethod.Get, ChannelTagsPath(contextId, linkedId), null, ct);

        /// <summary>Creates one assignment. Reuse its ID when retrying the same assignment.</summary>
        public async Task<CallChannelTag> AddChannelTag(Guid contextId, string linkedId, CallChannelTagDraft draft, CancellationToken ct)
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, ChannelTagsPath(contextId, linkedId))
            { Content = JsonContent.Create(draft, options: _json) };
            return await Request<CallChannelTag>(message, ct)
                ?? throw new InvalidOperationException("The service did not confirm the assignment.");
        }

        /// <summary>Returns active tag names for up to 200 visible calls. Untagged calls are omitted.</summary>
        public Task<CallHistoryTags[]?> ReadHistoryTags(Guid contextId, IReadOnlyList<CallHistoryTagTarget> calls, CancellationToken ct)
            => ReadTags<CallHistoryTags[]>(HttpMethod.Post, $"Telephony/Call/ChannelTags/History?contextid={contextId:D}", calls, ct);

        private async Task<T?> ReadTags<T>(HttpMethod method, string path, object? body, CancellationToken ct)
            where T : class
        {
            using var message = new HttpRequestMessage(method, path);
            if (body != null)
                message.Content = JsonContent.Create(body, options: _json);
            using var response = await SendAsync(message, ct);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            await response.EnsureSuccess(ct);
            return await response.Content.ReadFromJsonAsync<T>(_json, ct)
                ?? throw new InvalidOperationException("The service did not return call tags.");
        }
    }

    public sealed class CallChannelTag
    {
        public Guid Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Server { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public DateTimeOffset? Expiration { get; set; }
    }

    public sealed class CallChannelTagDraft
    {
        public Guid Id { get; set; }
        public string Server { get; set; } = string.Empty;
        public string ChannelId { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public DateTimeOffset? Expiration { get; set; }
    }

    public sealed class CallHistoryTagTarget
    {
        public string LinkedId { get; set; } = string.Empty;
        public DateTimeOffset Start { get; set; }
    }

    public sealed class CallHistoryTags
    {
        public string LinkedId { get; set; } = string.Empty;
        public string[] Tags { get; set; } = Array.Empty<string>();
    }
}
