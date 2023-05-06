using Microsoft.Extensions.Logging;
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

        public Task<IEnumerable<EndPoint>> GetEndPoints(EndPointSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (parameters.Id.HasValue)
                query["id"] = parameters.Id.ToString();

            if (parameters.UserId.HasValue)
                query["userid"] = parameters.UserId.ToString();

            if (parameters.ContextId.HasValue)
                query["contextid"] = parameters.ContextId.ToString();

            if (parameters.Title != null)
            {
                query["title.text"] = parameters.Title.Text;
                query["title.exactmatch"] = parameters.Title.ExactMatch.ToString();
            }

            if (parameters.Description != null)
            {
                query["description.text"] = parameters.Description.Text;
                query["description.exactmatch"] = parameters.Description.ExactMatch.ToString();
            }

            if (parameters.Limit > 0)
                query["limit"] = parameters.Limit.ToString();

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<EndPoint>(message, cancellationToken);
        }
    }
}
