using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Sufficit.Net.Http;
using Sufficit.Operations;

namespace Sufficit.Client.Controllers
{
    /// <summary>Authenticated collaborator work commands and context-scoped history.</summary>
    public sealed class OperationalActivitiesControllerSection : AuthenticatedControllerSection
    {
        private readonly System.Text.Json.JsonSerializerOptions _json;
        private const string Endpoint = "/Operations/Activities";
        public OperationalActivitiesControllerSection(IAuthenticatedControllerBase cb) : base(cb) { _json = cb.Json; }

        /// <summary>Creates or updates one revision using a stable command identifier.</summary>
        public Task<OperationalActivityHistory?> Apply(ActivityTransitionRequest command, CancellationToken token)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            command.Validate();
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri(Endpoint, UriKind.Relative));
            message.Content = JsonContent.Create(command, null, _json);
            return Request<OperationalActivityHistory>(message, token);
        }

        /// <summary>Reads a bounded stable page including work without an assignee.</summary>
        public Task<IEnumerable<OperationalActivity>> Search(Guid contextId, Guid? afterId, int limit, CancellationToken token)
        {
            Validate(contextId, limit);
            if (afterId == Guid.Empty) throw new ArgumentException("Invalid activity cursor.");
            var query = $"contextId={contextId:D}&limit={limit.ToString(CultureInfo.InvariantCulture)}";
            if (afterId.HasValue) query += $"&afterId={afterId.Value:D}";
            return RequestMany<OperationalActivity>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Endpoint}?{query}", UriKind.Relative)), token);
        }

        /// <summary>Reads immutable assignment, deadline and lifecycle evidence.</summary>
        public Task<IEnumerable<OperationalActivityHistory>> History(Guid contextId, Guid activityId, long afterRevision, int limit, CancellationToken token)
        {
            Validate(contextId, limit);
            if (activityId == Guid.Empty || afterRevision < 0) throw new ArgumentException("Invalid activity or revision.");
            var query = $"contextId={contextId:D}&afterRevision={afterRevision.ToString(CultureInfo.InvariantCulture)}&limit={limit.ToString(CultureInfo.InvariantCulture)}";
            return RequestMany<OperationalActivityHistory>(new HttpRequestMessage(HttpMethod.Get, new Uri($"{Endpoint}/{activityId:D}/History?{query}", UriKind.Relative)), token);
        }

        private static void Validate(Guid contextId, int limit)
        {
            if (contextId == Guid.Empty || limit < 1 || limit > 200) throw new ArgumentException("An explicit context and bounded limit are required.");
        }
    }
}
