using Microsoft.AspNetCore.Authorization;
using Sufficit.Client.Controllers.Contacts;
using Sufficit.Contacts;
using Sufficit.EndPoints;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
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
            Avatar = new AvatarControllerSection(cb);
            Attribute = new AttributeControllerSection(cb);
        }

        public AvatarControllerSection Avatar { get; }

        public AttributeControllerSection Attribute { get; }

        public Task<IEnumerable<ContactWithAttributes>> Search(ContactSearchParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<ContactWithAttributes>(message, cancellationToken);
        }

        public Task<IEnumerable<ContactWithAttributes>> Search(string filter, int results = 10, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrWhiteSpace(filter))
                query["filter"] = filter;

            if (results > 0)
                query["results"] = results.ToString();

            var uri = new Uri($"{Controller}/search?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ContactWithAttributes>(message, cancellationToken);
        }

        public Task<Contact?> GetContact(Guid contactid, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();

            var uri = new Uri($"{Controller}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<Contact>(message, cancellationToken);
        }

        public async Task<Guid?> Update(ContactWithAttributes item, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);

            var response = await Request<EndPointResponse>(message, cancellationToken);

            if (response?.Data is System.Text.Json.JsonElement json && json.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                var guidString = json.GetString();
                if (Guid.TryParse(guidString, out var parsedGuid))
                    return parsedGuid;
            }
            return null;
        }

        public async Task<bool> CanUpdate(Guid contactid, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contactid)] = contactid.ToString();

            var uri = new Uri($"{Controller}/canupdate?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return (await RequestStruct<bool>(message, cancellationToken)) ?? false;
        }

        /// <summary>
        /// Retrieves the list of user-defined markers available for the current context.
        /// </summary>
        [Authorize]
        public async Task<IEnumerable<MarkerAttributeGroup>> GetUserMarkers(CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/usermarkers", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestManyStruct<MarkerAttributeGroup>(message, cancellationToken);
        }

        protected override string[]? AnonymousPaths { get; } = { $"{Controller}/contactid", $"{Controller}/byid" };
    }
}
