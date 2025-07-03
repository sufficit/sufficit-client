using Microsoft.AspNetCore.Authorization;
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

        #region CONTACTS
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

        public Task<Contact?> GetContact(Guid contactid, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<Contact>(message, cancellationToken);
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

        #endregion
        #region ATTRIBUTES

        /// <summary>
        /// Retrieves a collection of attributes associated with the specified contact.
        /// </summary>
        /// <remarks>This method sends an HTTP GET request to retrieve the attributes of a contact. Ensure
        /// that the provided  <paramref name="contactid"/> is valid and corresponds to an existing contact in the
        /// system.</remarks>
        /// <param name="contactid">The unique identifier of the contact whose attributes are to be retrieved.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. This allows the operation to be canceled if needed.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of 
        /// <see cref="Sufficit.Contacts.Attribute"/> objects associated with the specified contact.</returns>
        public Task<IEnumerable<Sufficit.Contacts.Attribute>> GetAttributes(Guid contactid, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/attributes";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<Sufficit.Contacts.Attribute>(message, cancellationToken);
        }

        /// <summary>
        /// Retrieves the first contact attribute that matches the specified search parameters.
        /// </summary>
        /// <remarks>This method sends an HTTP GET request to retrieve the first contact attribute that
        /// matches the  provided search parameters. The search parameters are converted to a query string and appended 
        /// to the request URI.</remarks>
        /// <param name="parameters">The search parameters used to filter contact attributes. Must not be <see langword="null"/>.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the first matching  <see
        /// cref="Sufficit.Contacts.Attribute"/> if found; otherwise, <see langword="null"/>.</returns>
        public Task<Sufficit.Contacts.Attribute?> GetFirstAttribute(ContactAttributeSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/attribute";
            var query = parameters.ToQueryString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<Sufficit.Contacts.Attribute>(message, cancellationToken);
        }

        /// <summary>
        /// Retrieves the value of a specified attribute for a given contact.
        /// </summary>
        /// <remarks>This method sends an HTTP GET request to retrieve the attribute value. Ensure that
        /// the provided <paramref name="contactid"/>, <paramref name="key"/>, and <paramref name="description"/> are
        /// valid and correspond to existing data in the system.</remarks>
        /// <param name="contactid">The unique identifier of the contact whose attribute value is being retrieved.</param>
        /// <param name="key">The key of the attribute to retrieve.</param>
        /// <param name="description">A description or additional context for the attribute being retrieved.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the value of the specified
        /// attribute as a string, or <see langword="null"/> if the attribute is not found.</returns>
        public async Task<string?> GetAttributeValue(Guid contactid, string key, string description, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/attribute/value";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();
            query[nameof(key)] = key;
            query[nameof(description)] = description;

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestString(message, cancellationToken);
        }

        /// <summary>
        /// Removes a specified attribute from a contact.
        /// </summary>
        /// <remarks>This method sends a DELETE request to remove the specified attribute from the
        /// contact. Ensure that the <paramref name="contactid"/>, <paramref name="key"/>, and <paramref
        /// name="description"/>  are valid and correspond to an existing attribute for the specified contact.</remarks>
        /// <param name="contactid">The unique identifier of the contact from which the attribute will be removed.</param>
        /// <param name="key">The key of the attribute to be removed.</param>
        /// <param name="description">A description of the attribute being removed.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns></returns>
        public async Task RemoveAttribute(Guid contactid, string key, string description, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}/attribute/value";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();
            query[nameof(key)] = key;
            query[nameof(description)] = description;

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            await Request(message, cancellationToken);
        }

        #endregion

        public async Task<bool> CanUpdate(Guid contactid, CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/canupdate";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return (await RequestStruct<bool>(message, cancellationToken)) ?? false;
        }


        public async Task<EndPointResponse?> Update (Guid contextid, Stream avatar, CancellationToken cancellationToken)
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

        /// <summary>
        /// Retrieves a collection of user markers from the server.
        /// </summary>
        /// <remarks>This method sends an HTTP GET request to the server to fetch user markers. The
        /// returned collection contains the markers as strings. If no markers are available,  the collection will be
        /// empty.</remarks>
        /// <param name="cancellationToken">A token that can be used to cancel the operation. If the operation is canceled,  the returned task will be
        /// in a canceled state.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains  an enumerable collection of
        /// strings representing the user markers.</returns>
        [Authorize]
        public async Task<IEnumerable<MarkerAttributeGroup>> GetUserMarkers(CancellationToken cancellationToken)
        {
            string requestEndpoint = $"{Controller}/usermarkers";
            var uri = new Uri(requestEndpoint, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestManyStruct<MarkerAttributeGroup>(message, cancellationToken);
        }

        protected override string[]? AnonymousPaths { get; } = { $"{Controller}/contactid", $"{Controller}/byid" };
    }
}
