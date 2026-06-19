using Sufficit.EndPoints;
using Sufficit.Net.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Contacts
{
    public sealed class AvatarControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = ContactsControllerSection.Controller;
        private const string Prefix = "/avatar";

        public AvatarControllerSection(IAuthenticatedControllerBase cb) : base(cb) { }

        /// <summary>
        /// Checks whether a specific avatar exists for the given context (HEAD, no body).
        /// Returns true if a specific avatar is found (HTTP 200), false otherwise (HTTP 204).
        /// </summary>
        public async Task<bool> Exists(Guid contextid, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contextid)] = contextid.ToString();

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Head, uri);

            using var response = await SendAsync(message, cancellationToken);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        /// <summary>
        /// Uploads a new avatar image (.jpg) for the given context.
        /// </summary>
        public async Task<EndPointResponse?> Update(Guid contextid, Stream avatar, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(contextid)] = contextid.ToString();

            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);

            var formData = new MultipartFormDataContent();
            formData.Add(new StreamContent(avatar), nameof(avatar), contextid.ToString("N") + ".jpg");
            message.Content = formData;

            return await Request<EndPointResponse>(message, cancellationToken);
        }
    }
}
