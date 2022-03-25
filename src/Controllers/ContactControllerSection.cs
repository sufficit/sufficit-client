using Microsoft.Extensions.Logging;
using Sufficit.Client.Controllers.Telephony;
using Sufficit.Client.Extensions;
using Sufficit.Contacts;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class ContactControllerSection
    {
        public const string Controller = "/contact";
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public ContactControllerSection(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IContact?> GetContact(Guid id, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/contact";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            var uri = new Uri($"{ requestEndpoint }?{ query }", UriKind.Relative);
            return await _httpClient.GetFromJsonAsync<Contact>(uri, cancellationToken);
        }

        public async Task<IEnumerable<IContact>> GetContacts(string filter, int results = 10, CancellationToken cancellationToken = default)
        {            
            string requestEndpoint = $"{Controller}/contacts";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrWhiteSpace(filter))
                query["filter"] = filter;

            if (results > 0)
                query["results"] = results.ToString();

            var uri = new Uri($"{ requestEndpoint }?{ query }", UriKind.Relative);
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<Contact>>(uri, cancellationToken);
            if (response != null) return response;             
            else return new Contact[] { };
        }
    }
}
