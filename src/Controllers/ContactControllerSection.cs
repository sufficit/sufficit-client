using Microsoft.Extensions.Logging;
using Sufficit.Contacts;
using Sufficit.Gateway.ReceitaNet;
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
    
        public async Task<Contact?> GetContact(Guid id, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var requestUri = new Uri($"{ requestEndpoint }?{ query }", UriKind.Relative);
            var response = await httpClient.GetAsync(requestUri, cancellationToken);
            await response.EnsureSuccess();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            var content = await response.Content.ReadFromJsonAsync<Contact>();
            if (content != null) return content;
            else return null;
        }

        public Task<IEnumerable<Contact>> Search(ContactSearchParameters parameters, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/search";

            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, jsonOptions);
            return RequestMany<Contact>(message, cancellationToken);
        }
        
        public async Task<IEnumerable<Contact>> Search(string filter, int results = 10, CancellationToken cancellationToken = default)
        {            
            string requestEndpoint = $"{Controller}/search";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrWhiteSpace(filter))
                query["filter"] = filter;

            if (results > 0)
                query["results"] = results.ToString();

            var requestUri = new Uri($"{ requestEndpoint }?{ query }", UriKind.Relative);            
            var response = await httpClient.GetAsync(requestUri, cancellationToken);
            await response.EnsureSuccess();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Array.Empty<Contact>();

            var content = await response.Content.ReadFromJsonAsync<IEnumerable<Contact>>(jsonOptions, cancellationToken);
            if (content != null) return content;
            else return Array.Empty<Contact>();                       
        }

        public async Task<IAttribute?> GetAttribute(ContactAttributeSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/attribute";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["ContactId"] = parameters.ContactId.ToString();
            query["Key"] = parameters.Key;
            query["Value"] = parameters.Value;
            query["ExactMatch"] = parameters.ExactMatch.ToString();

            var requestUri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var response = await httpClient.GetAsync(requestUri, cancellationToken);
            await response.EnsureSuccess();

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await response.Content.ReadFromJsonAsync<Sufficit.Contacts.Attribute?>(jsonOptions, cancellationToken);
        }
    }
}
