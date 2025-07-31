using Sufficit.Identity;
using Sufficit.Net.Http;
using System.Text.Json;
using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Identity
{
    /// <summary>
    /// Controller section for token management operations in the Identity system
    /// </summary>
    public sealed class TokenControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = IdentityControllerSection.Controller;
        private const string Prefix = "/token";
        private readonly JsonSerializerOptions _json;
        public TokenControllerSection(IAuthenticatedControllerBase cb) : base(cb) 
        {
            _json = cb.Json;
        }

        /// <summary>
        /// Gets the current access token for the authenticated user
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Current access token</returns>
        public async Task<string?> GetCurrentToken(CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestString(message, cancellationToken);
        }

        /// <summary>
        /// Gets all reference tokens for the authenticated user
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of persisted grants (tokens)</returns>
        public Task<IEnumerable<PersistedGrant>> GetTokens(CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/reference";
            var uri = new Uri(requestEndpoint, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<PersistedGrant>(message, cancellationToken);
        }

        /// <summary>
        /// Updates the description of a specific token
        /// </summary>
        /// <param name="key">Token key to update</param>
        /// <param name="description">New description for the token</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        public async Task UpdateTokenDescription(string key, string? description, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/description";

            var message = new HttpRequestMessage(HttpMethod.Post, requestEndpoint);

            // Try using JSON content instead of form-encoded content to avoid URL encoding issues
            var payload = new TokenDescriptionUpdateRequest()
            {
                Key = key,
                Description = description
            };

            message.Content = JsonContent.Create(payload, null, _json);
            await Request(message, cancellationToken);
        }

        /// <summary>
        /// Revokes (removes) a specific token
        /// </summary>
        /// <param name="key">Token key to revoke</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        public async Task RevokeToken(string key, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/reference";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["key"] = key;

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);

            await Request(message, cancellationToken);
        }

        /// <summary>
        /// Gets the authorization URL for requesting a new token
        /// </summary>
        /// <param name="returnUrl">Optional return URL after authorization</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authorization URL</returns>
        public async Task<string> GetTokenRequestLink(string? returnUrl = null, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = $"{Controller}{Prefix}/requestlink";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            
            if (!string.IsNullOrWhiteSpace(returnUrl))
                query["returnUrl"] = returnUrl;

            var uri = new Uri($"{requestEndpoint}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return (await RequestString(message, cancellationToken))!;
        }

        /// <summary>
        /// Redirects to the token request page (for use in browser scenarios)
        /// </summary>
        /// <param name="returnUrl">Optional return URL after authorization</param>
        /// <returns>Redirect URL</returns>
        public string GetTokenRequestUrl(string? returnUrl = null)
        {
            string requestEndpoint = $"{Controller}{Prefix}/request";
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            
            if (!string.IsNullOrWhiteSpace(returnUrl))
                query["returnUrl"] = returnUrl;

            // Retorna URL relativa que será usada pelo NavigationManager
            return $"{requestEndpoint}?{query}";
        }
    }
}
