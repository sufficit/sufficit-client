using Sufficit.Contacts;
using Sufficit.EndPoints;
using Sufficit.Net.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class ContactsControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/contact";

        private readonly JsonSerializerOptions _json;

        public ContactsControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        public Task<Contact?> GetContact(Guid contactid, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<Contact>(message, cancellationToken);
        }

        public Task<IEnumerable<Sufficit.Contacts.Attribute>> GetAttributes(Guid contactid, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/attributes";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<Sufficit.Contacts.Attribute>(message, cancellationToken);
        }

        public Task<IEnumerable<ContactWithAttributes>> Search(ContactSearchParameters parameters, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/search";

            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<ContactWithAttributes>(message, cancellationToken);
        }

        public Task<IEnumerable<ContactWithAttributes>> Search(string filter, int results = 10, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/search";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrWhiteSpace(filter))
                query["filter"] = filter;

            if (results > 0)
                query["results"] = results.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ContactWithAttributes>(message, cancellationToken);
        }

        public Task<Sufficit.Contacts.Attribute?> GetAttribute(ContactAttributeSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/attribute";
            var query = parameters.ToQueryString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<Sufficit.Contacts.Attribute>(message, cancellationToken);
        }

        public async Task<bool> CanUpdate(Guid contactid, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/canupdate";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return (await RequestStruct<bool>(message, cancellationToken)) ?? false;
        }

        public async Task<Guid?> Update(ContactWithAttributes item, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}";

            var uri = new Uri($"{requestEndpoint}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            var response = await Request<EndPointResponse>(message, cancellationToken);
            return (Guid?)response?.Data;
        }

        public async Task<EndPointResponse?> Update(Guid contextid, Stream avatar, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/avatar";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contextid)] = contextid.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);

            var formData = new MultipartFormDataContent();
            formData.Add(new StreamContent(avatar), nameof(avatar), contextid.ToString("N") + ".jpg");
            message.Content = formData;

            return await Request<EndPointResponse>(message, cancellationToken);
        }

        protected override string[]? AnonymousPaths { get; } = { "/contact" };
    }
}
