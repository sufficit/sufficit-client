using Sufficit.EndPoints;
using Sufficit.Net.Http;
using Sufficit.Notification;
using Sufficit.Exchange;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Notification
{
    public sealed class ContactControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = NotificationControllerSection.Controller;
        private const string Prefix = "/contact";

        private readonly JsonSerializerOptions _json;

        public ContactControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        /// <summary>
        ///     Validate a contact destination and channel
        /// </summary>
        public async Task<ContactValidationResponse?> Validate(ContactValidationRequest parameters, CancellationToken cancellationToken = default)
        {       
            var uri = new Uri($"{Controller}{Prefix}/validate", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);            
            var response = await Request<ContactValidationResponse>(message, cancellationToken);
            return response;
        }

        /// <summary>
        ///     Get contacts based on search parameters
        /// </summary>
        [Authorize]
        public Task<IEnumerable<Contact>> GetContacts(ContactSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return RequestMany<Contact>(message, cancellationToken);
        }

        /// <summary>
        ///     Get a specific contact by ID
        /// </summary>
        [Authorize]
        public Task<Contact?> GetContact(Guid id, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/{id}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<Contact>(message, cancellationToken);
        }

        /// <summary>
        ///     Create or update a contact
        /// </summary>
        [Authorize]
        public Task UpdateContact(Contact contact, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(contact, null, _json);
            return Request(message, cancellationToken);
        }

        /// <summary>
        ///     Remove a contact and all its subscriptions
        /// </summary>
        [Authorize]
        public Task RemoveContact(Guid id, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/{id}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        /// <summary>
        ///     Check if a contact is valid (not blocked/invalid)
        /// </summary>
        [Authorize]
        public async Task<bool> IsValidContact(string destination, TChannel channel = default, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/isvalid?destination={Uri.EscapeDataString(destination)}&channel={channel}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            var result = await RequestStruct<bool>(message, cancellationToken);
            return result ?? false;
        }

        /// <summary>
        ///     Get the reason why a contact is invalid
        /// </summary>
        [Authorize]
        public async Task<string?> GetCause(string destination, TChannel channel = default, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/getcause?destination={Uri.EscapeDataString(destination)}&channel={channel}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestString(message, cancellationToken);
        }

        protected override string[]? AnonymousPaths { get; } = { $"{Controller}{Prefix}/validate" };
    }
}
