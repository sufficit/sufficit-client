using Microsoft.AspNetCore.Authorization;
using Sufficit.Net.Http;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyChromeExtensionControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/ChromeExtension";

        public TelephonyChromeExtensionControllerSection(IAuthenticatedControllerBase cb) : base(cb) { }

        /// <summary>
        /// Returns the SIP configuration for the current user's Chrome softphone,
        /// including runtime parameters such as <see cref="SipRuntimeConfig.ReconnectWatchdogErrorWindowMs"/>.
        /// </summary>
        [Authorize]
        public Task<ChromeExtensionConfig?> GetConfig(CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/Config", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<ChromeExtensionConfig>(message, cancellationToken);
        }

        /// <summary>
        /// Lists all available endpoints (extensions) for the authenticated user.
        /// Managers with TelephonyClientDirective on Guid.Empty see endpoints across all their known contexts.
        /// Optionally pass <paramref name="contextId"/> to filter by a specific context.
        /// </summary>
        [Authorize]
        public Task<IEnumerable<EndPoint>> GetEndPoints(CancellationToken cancellationToken, Guid? contextId = null)
        {
            var uriString = contextId.HasValue && contextId.Value != Guid.Empty
                ? $"{Controller}{Prefix}/EndPoints?contextId={contextId.Value}"
                : $"{Controller}{Prefix}/EndPoints";
            var uri = new Uri(uriString, UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<EndPoint>(message, cancellationToken);
        }

        /// <summary>
        /// Returns the preferred endpoint configured for the softphone, if any
        /// </summary>
        [Authorize]
        public Task<EndPoint?> GetPreferred(CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/Preferred", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<EndPoint>(message, cancellationToken);
        }

        /// <summary>
        /// Sets the preferred endpoint for the softphone / Chrome Extension
        /// </summary>
        [Authorize]
        public Task SetPreferred(Guid endpointId, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["endpointId"] = endpointId.ToString();
            var uri = new Uri($"{Controller}{Prefix}/Preferred?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            return Request(message, cancellationToken);
        }

        /// <summary>
        /// Returns all softphone settings for the authenticated user
        /// </summary>
        [Authorize]
        public Task<IEnumerable<ChromeExtensionSetting>> GetSettings(CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/Settings", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<ChromeExtensionSetting>(message, cancellationToken);
        }

        /// <summary>
        /// Creates or updates a single softphone setting. Null value removes the key.
        /// </summary>
        [Authorize]
        public Task SetSetting(string key, string? value, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["key"] = key;
            if (value != null) query["value"] = value;
            var uri = new Uri($"{Controller}{Prefix}/Settings?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            return Request(message, cancellationToken);
        }

    }
}
