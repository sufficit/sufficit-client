using Microsoft.Extensions.Logging;
using Sufficit.Contacts;
using Sufficit.Gateway.ReceitaNet;
using Sufficit.Identity;
using Sufficit.Logging;
using Sufficit.Telephony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class ContactControllerSection : ControllerSection
    {
        public const string Controller = "/contact";

        public ContactControllerSection(APIClientService service) : base(service) { }
    
        public Task<Contact?> GetContact(Guid contactid, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contactid"] = contactid.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<Contact>(message, cancellationToken);
        }

        public Task<IEnumerable<Contact>> Search(ContactSearchParameters parameters, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/search";

            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return RequestMany<Contact>(message, cancellationToken);
        }
        
        public Task<IEnumerable<Contact>> Search(string filter, int results = 10, CancellationToken cancellationToken = default)
        {            
            string requestEndpoint = $"{Controller}/search";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrWhiteSpace(filter))
                query["filter"] = filter;

            if (results > 0)
                query["results"] = results.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<Contact>(message, cancellationToken);
        }

        public Task<Sufficit.Contacts.Attribute?> GetAttribute(ContactAttributeSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/attribute";
            var query = parameters.ToQueryString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<Sufficit.Contacts.Attribute>(message, cancellationToken);
        }
    }
}
