using Microsoft.AspNetCore.Authorization;
using Sufficit.Exchange;
using Sufficit.Exchange.Templates;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Exchange
{
    public sealed class MessageTemplateControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = ExchangeControllerSection.Controller;
        private const string Prefix = "/templates";

        private readonly JsonSerializerOptions _json;

        public MessageTemplateControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        /// <summary>
        /// Search message templates
        /// </summary>
        [Authorize(Roles = $"{Sufficit.Identity.ManagerRole.NormalizedName},{Sufficit.Identity.AdministratorRole.NormalizedName}")]
        public Task<IEnumerable<MessageTemplate>> Search(MessageTemplateSearchParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/search", UriKind.Relative);
            var content = JsonContent.Create(parameters, null, _json);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content
            };
            return RequestMany<MessageTemplate>(message, cancellationToken);
        }

        /// <summary>
        /// Get message template by ID
        /// </summary>
        [Authorize(Roles = $"{Sufficit.Identity.ManagerRole.NormalizedName},{Sufficit.Identity.AdministratorRole.NormalizedName}")]
        public Task<MessageTemplate?> GetById(Guid id, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/ById/{id}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<MessageTemplate>(message, cancellationToken);
        }

        /// <summary>
        /// Create or update message template
        /// </summary>
        [Authorize(Roles = $"{Sufficit.Identity.ManagerRole.NormalizedName},{Sufficit.Identity.AdministratorRole.NormalizedName}")]
        public Task<MessageTemplate> CreateOrUpdate(MessageTemplate template, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/CreateOrUpdate", UriKind.Relative);
            var content = JsonContent.Create(template, null, _json);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content
            };
            return Request<MessageTemplate>(message, cancellationToken);
        }

        /// <summary>
        /// Delete message template
        /// </summary>
        [Authorize(Roles = $"{Sufficit.Identity.ManagerRole.NormalizedName},{Sufficit.Identity.AdministratorRole.NormalizedName}")]
        public Task<bool> Delete(Guid id, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/ById/{id}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request<bool>(message, cancellationToken);
        }

        /// <summary>
        /// Get approved templates by context
        /// </summary>
        [Authorize(Roles = $"{Sufficit.Identity.ManagerRole.NormalizedName},{Sufficit.Identity.AdministratorRole.NormalizedName}")]
        public Task<IEnumerable<MessageTemplate>> GetApprovedByContext(Guid contextId, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/Approved/{contextId}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<MessageTemplate>(message, cancellationToken);
        }

        /// <summary>
        /// Get approved template by title
        /// </summary>
        [Authorize(Roles = $"{Sufficit.Identity.ManagerRole.NormalizedName},{Sufficit.Identity.AdministratorRole.NormalizedName}")]
        public Task<MessageTemplate?> GetApprovedByTitle(Guid contextId, string title, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/Approved/{contextId}/{title}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<MessageTemplate>(message, cancellationToken);
        }

        /// <summary>
        /// Set template approval status
        /// </summary>
        [Authorize(Roles = $"{Sufficit.Identity.ManagerRole.NormalizedName},{Sufficit.Identity.AdministratorRole.NormalizedName}")]
        public Task<bool> SetApproval(Guid id, bool approved, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/SetApproval/{id}/{approved}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            return Request<bool>(message, cancellationToken);
        }
    }
}
