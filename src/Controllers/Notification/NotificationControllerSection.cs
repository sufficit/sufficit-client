using Sufficit.Notification;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Notification
{
    public sealed class NotificationControllerSection : ControllerSection
    {
        public const string Controller = "/notification";

        public NotificationControllerSection(APIClientService service) : base(service) 
        {
            Contact = new NotificationContactControllerSection(service);
        }

        public NotificationContactControllerSection Contact { get; }

        public Task<IEnumerable<BoardNotification>> GetNotifications(CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/boardnotifications";
            //var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            //query["id"] = id.ToString();

            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<BoardNotification>(message, cancellationToken);
        }

        public Task<IEnumerable<EventInfo>> GetEvents(CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/events";
            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<EventInfo>(message, cancellationToken);
        }

        public Task Subscribe(SubscribeRequest request, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/subscribe", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, jsonOptions);
            return Request(message, cancellationToken);
        }
    }
}
