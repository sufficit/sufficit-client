using Sufficit.Contacts;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Contacts
{
    public sealed class AttributeControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = ContactsControllerSection.Controller;
        private const string Prefix = "/attribute";

        private readonly JsonSerializerOptions _json;

        public AttributeControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        /// <summary>
        /// Retrieves a collection of contact attributes matching the specified search parameters.
        /// </summary>
        public Task<IEnumerable<ContactAttribute>> Search(AttributeWithKeysSearchParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/attributes", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<ContactAttribute>(message, cancellationToken);
        }

        /// <summary>
        /// Retrieves the first contact attribute matching the specified search parameters.
        /// </summary>
        public Task<ContactAttribute?> GetFirst(AttributeWithKeysSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<ContactAttribute>(message, cancellationToken);
        }

        /// <summary>
        /// Retrieves the string value of a specific attribute for a contact.
        /// </summary>
        public async Task<string?> GetValue(Guid contactid, string key, string description, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();
            query[nameof(key)] = key;
            query[nameof(description)] = description;

            var uri = new Uri($"{Controller}{Prefix}/value?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestString(message, cancellationToken);
        }

        /// <summary>
        /// Removes a specific attribute from a contact.
        /// </summary>
        public async Task Remove(Guid contactid, string key, string description, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();
            query[nameof(key)] = key;
            query[nameof(description)] = description;

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            await Request(message, cancellationToken);
        }

        /// <summary>
        /// Creates or updates a contact attribute.
        /// </summary>
        public async Task CreateOrUpdate(Guid contactid, Sufficit.Contacts.Attribute attribute, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(attribute, null, _json);

            await Request(message, cancellationToken);
        }
    }
}
