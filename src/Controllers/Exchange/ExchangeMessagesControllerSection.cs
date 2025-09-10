using Microsoft.AspNetCore.Authorization;
using Sufficit.Exchange;
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
    public sealed class ExchangeMessagesControllerSection : AuthenticatedControllerSection, IMessagesController
    {
        private const string Controller = ExchangeControllerSection.Controller;
        private const string Prefix = "/messages";

        private readonly JsonSerializerOptions _json;

        public ExchangeMessagesControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        /// <inheritdoc cref="IMessagesController.GetDetails(MessageDetailsSearchParameters, CancellationToken) "/>
        [Authorize(Roles = $"{Sufficit.Identity.ManagerRole.NormalizedName},{Sufficit.Identity.AdministratorRole.NormalizedName}")]
        public Task<IEnumerable<MessageDetails>> GetDetails (MessageDetailsSearchParameters parameters, CancellationToken cancellationToken)
        {          
            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var content = JsonContent.Create(parameters, null, _json);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content
            };
            return RequestMany<MessageDetails>(message, cancellationToken);
        }

        /// <inheritdoc cref="IMessagesController.GetDetails(Guid, bool?, CancellationToken) "/>
        [Authorize(Roles = $"{Sufficit.Identity.ManagerRole.NormalizedName},{Sufficit.Identity.AdministratorRole.NormalizedName}")]
        public Task<MessageDetails?> GetDetails(Guid id, bool? content = default, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();

            if (content.HasValue)
                query["content"] = content.ToString()!.ToLower();

            var uri = new Uri($"{Controller}{Prefix}/ByMessageId?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<MessageDetails>(message, cancellationToken);
        }

        /// <summary>
        /// Get messages by reference ID
        /// </summary>
        [Authorize(Roles = $"{Sufficit.Identity.ManagerRole.NormalizedName},{Sufficit.Identity.AdministratorRole.NormalizedName}")]
        public Task<IEnumerable<MessageDetails>?> GetByReferenceId(Guid id, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/ByReferenceId/{id}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<MessageDetails>(message, cancellationToken);
        }
    }
}
