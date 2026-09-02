using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Gateway
{
    /// <summary>
    /// Public Gravatar lookups (avatar and profile) exposed by the Endpoints gateway area.
    /// </summary>
    /// <remarks>
    /// Absence is reported as null: the endpoints answer 404 when the e-mail or hash
    /// has no Gravatar avatar / public profile, and this section maps that to null.
    /// </remarks>
    public sealed class GravatarControllerSection : AuthenticatedControllerSection
    {
        private const string ControllerPath = "/Gateway/Gravatar";

        private readonly JsonSerializerOptions _json;

        public GravatarControllerSection(IAuthenticatedControllerBase cb)
            : base(cb)
        {
            _json = cb.Json;
        }

        /// <summary>
        /// Gets the Gravatar avatar image for an e-mail address.
        /// </summary>
        /// <returns>image bytes and metadata, or null when the e-mail has no Gravatar</returns>
        public Task<GravatarAvatarImage?> GetAvatarByEmailAsync(
            string email,
            uint? size = null,
            CancellationToken cancellationToken = default)
        {
            var query = "?email=" + Uri.EscapeDataString((email ?? string.Empty).Trim());
            if (size.HasValue && size.Value > 0)
                query += "&size=" + size.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

            return RequestAvatarAsync(
                new HttpRequestMessage(HttpMethod.Get, ControllerPath + "/avatar" + query),
                cancellationToken);
        }

        /// <summary>
        /// Gets the Gravatar avatar image for a MD5 or SHA-256 hash.
        /// </summary>
        /// <returns>image bytes and metadata, or null when the hash has no Gravatar</returns>
        public Task<GravatarAvatarImage?> GetAvatarByHashAsync(
            string hash,
            uint? size = null,
            CancellationToken cancellationToken = default)
        {
            var query = string.Empty;
            if (size.HasValue && size.Value > 0)
                query = "?size=" + size.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

            return RequestAvatarAsync(
                new HttpRequestMessage(HttpMethod.Get, ControllerPath + "/avatar/" + Uri.EscapeDataString((hash ?? string.Empty).Trim()) + query),
                cancellationToken);
        }

        /// <summary>
        /// Gets the public Gravatar profile for an e-mail address.
        /// </summary>
        /// <returns>public profile, or null when the e-mail has no public profile</returns>
        public Task<GravatarProfileView?> GetProfileByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            var query = "?email=" + Uri.EscapeDataString((email ?? string.Empty).Trim());
            return RequestOptionalAsync<GravatarProfileView>(
                new HttpRequestMessage(HttpMethod.Get, ControllerPath + "/profile" + query),
                cancellationToken);
        }

        /// <summary>
        /// Gets the public Gravatar profile for a MD5 or SHA-256 hash.
        /// </summary>
        /// <returns>public profile, or null when the hash has no public profile</returns>
        public Task<GravatarProfileView?> GetProfileByHashAsync(
            string hash,
            CancellationToken cancellationToken = default)
        {
            return RequestOptionalAsync<GravatarProfileView>(
                new HttpRequestMessage(HttpMethod.Get, ControllerPath + "/profile/" + Uri.EscapeDataString((hash ?? string.Empty).Trim())),
                cancellationToken);
        }

        /// <summary>
        /// Combined presence lookup for an e-mail: avatar and public profile in one call.
        /// </summary>
        public Task<GravatarSearchResultView?> SearchByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            var query = "?email=" + Uri.EscapeDataString((email ?? string.Empty).Trim());
            return RequestOptionalAsync<GravatarSearchResultView>(
                new HttpRequestMessage(HttpMethod.Get, ControllerPath + "/search" + query),
                cancellationToken);
        }

        /// <summary>
        /// Sends an avatar request; maps 404 (no Gravatar for the input) to null instead of throwing.
        /// </summary>
        private async Task<GravatarAvatarImage?> RequestAvatarAsync(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            using var response = await SendAsync(message, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            await response.EnsureSuccess(cancellationToken);

#if NETSTANDARD2_0
            var content = await response.Content.ReadAsByteArrayAsync();
#else
            var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
#endif
            if (content == null || content.Length == 0)
                return null;

            return new GravatarAvatarImage
            {
                Content = content,
                ContentType = response.Content.Headers.ContentType?.MediaType,
                LastModified = response.Content.Headers.LastModified?.UtcDateTime
            };
        }

        /// <summary>
        /// Sends a JSON request; maps 404 (absence) to null instead of throwing.
        /// </summary>
        private async Task<T?> RequestOptionalAsync<T>(HttpRequestMessage message, CancellationToken cancellationToken)
            where T : class
        {
            using var response = await SendAsync(message, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            await response.EnsureSuccess(cancellationToken);

            if (response.StatusCode == HttpStatusCode.NoContent)
                return null;

#if NETSTANDARD2_0
            using var stream = await response.Content.ReadAsStreamAsync();
#else
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
#endif
            return await JsonSerializer.DeserializeAsync<T>(stream, _json, cancellationToken);
        }
    }

    /// <summary>
    /// Avatar image bytes plus the metadata exposed by the gateway endpoint.
    /// </summary>
    public sealed class GravatarAvatarImage
    {
        /// <summary>
        /// Image content (usually JPEG or PNG).
        /// </summary>
        public byte[] Content { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Media type of the image.
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Last modification reported by Gravatar, for caching purposes.
        /// </summary>
        public DateTime? LastModified { get; set; }
    }

    /// <summary>
    /// A public photo declared by a Gravatar profile.
    /// </summary>
    public sealed class GravatarPhotoView
    {
        public string Value { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;
    }

    /// <summary>
    /// A verified/declared account (GitHub, Twitter, ...) of a Gravatar profile.
    /// </summary>
    public sealed class GravatarAccountView
    {
        public string Url { get; set; } = string.Empty;

        public string Shortname { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
    }

    /// <summary>
    /// Public profile entry returned by the gateway profile endpoints.
    /// </summary>
    public sealed class GravatarProfileView
    {
        public string Hash { get; set; } = string.Empty;

        public string RequestHash { get; set; } = string.Empty;

        public string ProfileUrl { get; set; } = string.Empty;

        public string PreferredUsername { get; set; } = string.Empty;

        public string? DisplayName { get; set; }

        public string? AboutMe { get; set; }

        public string? CurrentLocation { get; set; }

        public string? ThumbnailUrl { get; set; }

        public IReadOnlyList<GravatarPhotoView> Photos { get; set; } = Array.Empty<GravatarPhotoView>();

        public IReadOnlyList<GravatarAccountView> Accounts { get; set; } = Array.Empty<GravatarAccountView>();
    }

    /// <summary>
    /// Combined presence result for an e-mail address, returned by the gateway search endpoint.
    /// </summary>
    public sealed class GravatarSearchResultView
    {
        /// <summary>
        /// Normalized e-mail that was searched.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// SHA-256 hash of the e-mail (modern Gravatar identifier).
        /// </summary>
        public string Hash { get; set; } = string.Empty;

        /// <summary>
        /// Legacy MD5 hash of the e-mail.
        /// </summary>
        public string HashMd5 { get; set; } = string.Empty;

        public bool HasAvatar { get; set; }

        public bool HasProfile { get; set; }

        public GravatarAvatarImage? Avatar { get; set; }

        public GravatarProfileView? Profile { get; set; }
    }
}
