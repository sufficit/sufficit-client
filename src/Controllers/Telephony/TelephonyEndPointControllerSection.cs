using Microsoft.Extensions.Logging;
using Sufficit.Contacts;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyEndPointControllerSection : ControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/endpoint";

        public TelephonyEndPointControllerSection(APIClientService service) : base(service) { }  

        public Task<IEnumerable<EndPoint>> GetEndPoints(EndPointSearchParameters parameters, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}{Prefix}/search";

            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return RequestMany<EndPoint>(message, cancellationToken);
        }
    }
}
