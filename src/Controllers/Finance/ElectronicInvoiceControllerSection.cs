using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Sufficit.Finance;
using Sufficit.EndPoints;
using Sufficit.Identity;
using Sufficit.Json;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Finance
{
    public sealed class ElectronicInvoiceControllerSection : AuthenticatedControllerSection, IElectronicInvoiceController
    {
        private const string Controller = FinanceControllerSection.Controller;
        private const string Prefix = $"/{nameof(ElectronicInvoice)}";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public ElectronicInvoiceControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        /// <inheritdoc cref="IElectronicInvoiceController.ById(Guid, CancellationToken) "/>
        public Task<ElectronicInvoice?> ById(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogTrace("by id: {id}", id);
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            var uri = new Uri($"{Controller}{Prefix}/{nameof(ById)}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<ElectronicInvoice>(message, cancellationToken);
        }

        /// <inheritdoc cref="IElectronicInvoiceController.AddOrUpdate(ElectronicInvoice, CancellationToken) "/>
        [Authorize(Roles = $"{AdministratorRole.NormalizedName},{ManagerRole.NormalizedName}")]
        public async Task<ElectronicInvoice> AddOrUpdate(ElectronicInvoice item, CancellationToken cancellationToken)
        {
            _logger.LogTrace("add or update item: {item}", item.ToJsonOrDefault());
            var uri = new Uri($"{Controller}{Prefix}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            var response = await Request<EndPointResponse<ElectronicInvoice>>(message, cancellationToken);
            if (response is null || !response.Success)
                throw new RequestException("invalid response");

            return response.Data;
        }


        /// <inheritdoc cref="IElectronicInvoiceController.Search(ElectronicInvoiceSearchParameters, CancellationToken) "/>
        [Authorize(Roles = $"{AdministratorRole.NormalizedName},{ManagerRole.NormalizedName}")]
        public Task<IEnumerable<ElectronicInvoice>> Search(ElectronicInvoiceSearchParameters parameters, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}{Prefix}/search", UriKind.Relative);
            var content = JsonContent.Create(parameters, null, _json);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content
            };
            return RequestMany<ElectronicInvoice>(message, cancellationToken);
        }


        [Authorize(Roles = $"{AdministratorRole.NormalizedName}")]
        public async Task<int> Remove(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogTrace("remove by id: {id}", id);
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id.ToString();
            var uri = new Uri($"{Controller}{Prefix}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            var response = await Request<EndPointResponse>(message, cancellationToken);
            return response?.Success ?? false ? 1 : 0;
        }

        protected override string[]? AnonymousPaths { get; } = { $"{Controller}{Prefix}/{nameof(ById)}" };
    }
}
