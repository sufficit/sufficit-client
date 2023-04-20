using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sufficit.Telephony;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyBalanceControllerSection : ControllerSection, BalanceControllerInterface
    {
        private const string Controller = TelephonyControllerSection.Controller;

        public TelephonyBalanceControllerSection(APIClientService service) : base(service) { }

        /// <inheritdoc cref="BalanceControllerInterface.Notify"/>
        public async Task Notify(BalanceNotifyRequest request, CancellationToken cancellationToken)
        {
            logger.LogTrace("notifying: {contextid} => {force}", request.ContextId, request.Force);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = request.ContextId.ToString();
            query["force"] = request.Force.ToString();

            var uri = new Uri($"{Controller}/balance?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Head, uri);            
            await Request(message, cancellationToken);
        }

        /// <inheritdoc cref="BalanceControllerInterface.Patch"/>
        [Authorize(Roles = "telephonyadmin,salesmanager")]
        public async Task Patch(BalancePatchRequest request, CancellationToken cancellationToken)
        {
            logger.LogTrace("patching: {contextid} => {limit}", request.ContextId, request.Limit);

            var uri = new Uri($"{Controller}/balance", UriKind.Relative);
#if NET5_0_OR_GREATER
            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
#else
            var message = new HttpRequestMessage(HttpMethod.Put, uri);
#endif
            message.Content = JsonContent.Create(request, null, jsonOptions);
            await Request(message, cancellationToken);
        }

        /// <inheritdoc cref="BalanceControllerInterface.Get"/>
        public async Task<Balance?> Get(Guid contextId, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}/balance?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await Request<Balance>(message, cancellationToken);
        }

        /// <inheritdoc cref="BalanceControllerInterface.GetAmount"/>
        public async Task<decimal?> GetAmount(Guid contextId, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}/balance/amount?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestStruct<decimal>(message, cancellationToken);
        }
    }
}
