using Microsoft.Extensions.Logging;
using Sufficit.Contacts;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyCarrierControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/carrier";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public TelephonyCarrierControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        public async Task<IEnumerable<IIdTitlePair>> Get(CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            var contacts = await RequestMany<Contact>(message, cancellationToken);
            return contacts.Cast<IIdTitlePair>();
        }

        public Task<IEnumerable<ContactWithAttributes>> Search(string? filter, uint results = 20, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrWhiteSpace(filter))
                query["filter"] = filter;

            query["results"] = results.ToString();

            var uri = new Uri($"{Controller}{Prefix}/search?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ContactWithAttributes>(message, cancellationToken);
        }

        public Task<IEnumerable<ContactWithAttributes>> Search(ContactSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("carrier search parameters: {parameters}", parameters);

            var uri = new Uri($"{Controller}{Prefix}/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(parameters, null, _json)
            };

            return RequestMany<ContactWithAttributes>(message, cancellationToken);
        }

        public Task<ContactWithAttributes?> GetContactWithAttributes(Guid contactid, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();

            var uri = new Uri($"{Controller}{Prefix}/contactwithattributes?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<ContactWithAttributes>(message, cancellationToken);
        }

        public Task AddOrUpdateContact(ContactWithAttributes item, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/contact", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(item, null, _json)
            };

            return Request(message, cancellationToken);
        }

        public Task UpdateAttribute(Guid contactid, Sufficit.Contacts.Attribute attribute, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();

            var uri = new Uri($"{Controller}{Prefix}/attribute?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(attribute, null, _json)
            };

            return Request(message, cancellationToken);
        }

        public Task RemoveAttribute(Guid contactid, string key, string description = "", CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();
            query[nameof(key)] = key;
            query[nameof(description)] = description;

            var uri = new Uri($"{Controller}{Prefix}/attribute?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }
    }
}