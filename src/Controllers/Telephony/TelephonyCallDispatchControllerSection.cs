using Microsoft.Extensions.Logging;
using Sufficit.Net.Http;
using Sufficit.Telephony.CallDispatch;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyCallDispatchControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Prefix = "/calldispatch";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public TelephonyCallDispatchControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
            _json = cb.Json;
        }

        public Task<CallDispatchStartResult?> Start(CallDispatchRequest request, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/start", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(request, null, _json)
            };

            return Request<CallDispatchStartResult>(message, cancellationToken);
        }

        public Task<CallDispatchExecution?> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(id)] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/byid?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<CallDispatchExecution>(message, cancellationToken);
        }

        public Task<IEnumerable<CallDispatchAttempt>> GetAttempts(Guid id, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(id)] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/attempts?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<CallDispatchAttempt>(message, cancellationToken);
        }

        public Task<IEnumerable<CallDispatchExecution>> SearchExecutions(CallDispatchExecutionSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("call dispatch execution search parameters: {parameters}", parameters);

            var uri = new Uri($"{Controller}{Prefix}/executions/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(parameters, null, _json)
            };

            return RequestMany<CallDispatchExecution>(message, cancellationToken);
        }

        public Task<IEnumerable<CallDispatchConfiguration>> SearchConfigurations(CallDispatchConfigurationSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("call dispatch configuration search parameters: {parameters}", parameters);

            var uri = new Uri($"{Controller}{Prefix}/configurations/search", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(parameters, null, _json)
            };

            return RequestMany<CallDispatchConfiguration>(message, cancellationToken);
        }

        public Task<CallDispatchConfiguration?> GetConfigurationById(Guid id, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(id)] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/configuration?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<CallDispatchConfiguration>(message, cancellationToken);
        }

        public Task<CallDispatchConfiguration?> AddOrUpdateConfiguration(CallDispatchConfiguration item, CancellationToken cancellationToken = default)
        {
            var uri = new Uri($"{Controller}{Prefix}/configuration", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = JsonContent.Create(item, null, _json)
            };

            return Request<CallDispatchConfiguration>(message, cancellationToken);
        }

        public Task DeleteConfiguration(Guid id, CancellationToken cancellationToken = default)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(id)] = id.ToString();

            var uri = new Uri($"{Controller}{Prefix}/configuration?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        // TODO: add cancel, reprocess and clear-history client calls after the backend contract is implemented.
    }
}
