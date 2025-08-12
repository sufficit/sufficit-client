using Microsoft.AspNetCore.Authorization;
using Sufficit.Net.Http;
using Sufficit.Notification;
using Sufficit.Identity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Notification
{
    public sealed class NotificationControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/notification";

        private readonly JsonSerializerOptions _json;

        public NotificationControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            Contact = new ContactControllerSection(cb);

            _json = cb.Json;
        }

        public ContactControllerSection Contact { get; }

        [Authorize]
        public Task<IEnumerable<BoardNotification>> GetNotifications(CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/boardnotifications";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<BoardNotification>(message, cancellationToken);
        }

        /// <summary>
        ///     Get Available Events for notifications, UI use and filter
        /// </summary>
        public Task<IEnumerable<EventInfo>> GetEvents(CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/events";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<EventInfo>(message, cancellationToken);
        }

        public Task Subscribe (SubscribeRequest request, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/subscribe", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            return Request(message, cancellationToken);
        }

        /// <summary>
        ///     Send a board notification
        /// </summary>
        [Authorize]
        public Task Notify(BoardNotification item, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/boardnotify", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            return Request(message, cancellationToken);
        }

        /// <summary>
        ///     Clean up old notification data (Administrator only)
        /// </summary>
        [Authorize(Roles = AdministratorRole.NormalizedName)]
        public Task<int?> CleanUp(CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/cleanup";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestStruct<int>(message, cancellationToken);
        }

        protected override string[]? AnonymousPaths { get; } = { 
            $"{Controller}/events", 
            $"{Controller}/subscribe"
        };
    }
}
