using Sufficit.Identity;
using Sufficit.Notification;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class NotificationControllerSection : ControllerSection
    {
        public const string Controller = "/notification";

        public NotificationControllerSection(APIClientService service) : base(service) { }

        public Task<IEnumerable<BoardNotification>> GetNotifications(CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/boardnotifications";
            //var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            //query["id"] = id.ToString();

            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<BoardNotification>(message, cancellationToken);
        }
    }
}
