using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Sufficit.Identity;
using Sufficit.Net.Http;
using Sufficit.Sales;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    /// <summary>
    ///     Telephony Billing
    /// </summary>
    public sealed class TelephonyBillingControllerSection : AuthenticatedControllerSection, BalanceControllerInterface
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Section = "billing";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public TelephonyBillingControllerSection(IAuthenticatedControllerBase cb) : base(cb) 
        {
            _json = cb.Json;
            _logger = cb.Logger;
        }

        #region COST METHODS

        /// <summary>
        /// Update billing cost
        /// </summary>
        [Authorize(Roles = $"{TelephonyAdminRole.NormalizedName},{SalesManagerRole.NormalizedName}")]
        public async Task<BillingCost> CostUpdate(BillingCost item, CancellationToken cancellationToken)
        {
            _logger.LogTrace("updating cost: {cost}", item.Cost);

            var uri = new Uri($"{Controller}/{Section}/cost", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(item, null, _json);
            return await Request<BillingCost>(message, cancellationToken) ?? item;
        }

        /// <summary>
        /// Delete billing cost
        /// </summary>
        [Authorize(Roles = $"{TelephonyAdminRole.NormalizedName},{SalesManagerRole.NormalizedName}")]
        public async Task<BillingCost> CostDelete(BillingCost item, CancellationToken cancellationToken)
        {
            _logger.LogTrace("deleting cost: {cost}", item.Cost);

            var uri = new Uri($"{Controller}/{Section}/cost", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            message.Content = JsonContent.Create(item, null, _json);
            return await Request<BillingCost>(message, cancellationToken) ?? item;
        }

        /// <summary>
        /// Get record cost
        /// </summary>
        [Authorize(Roles = $"{TelephonyAdminRole.NormalizedName},{SalesManagerRole.NormalizedName}")]
        public async Task<decimal?> GetRecordCost(Guid contextid, string protocol, CancellationToken cancellationToken)
        {
            _logger.LogTrace("getting record cost: {contextid}, {protocol}", contextid, protocol);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextid.ToString();
            query["protocol"] = protocol;

            var uri = new Uri($"{Controller}/{Section}/cost?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestStruct<decimal>(message, cancellationToken);
        }

        #endregion

        #region BILLING RECORD METHODS

        /// <summary>
        /// Get billing record
        /// </summary>
        public async Task<CallBillingRecord?> GetBillingRecord(Guid contextid, string protocol, CancellationToken cancellationToken)
        {
            _logger.LogTrace("getting billing record: {contextid}, {protocol}", contextid, protocol);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextid.ToString();
            query["protocol"] = protocol;

            var uri = new Uri($"{Controller}/{Section}/record?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await Request<CallBillingRecord>(message, cancellationToken);
        }

        /// <summary>
        /// Get billing records using search parameters
        /// </summary>
        public Task<IEnumerable<CallBillingRecord>> GetBillingRecords(BillingSearchParameters parameters, CancellationToken cancellationToken)
        {
            _logger.LogTrace("getting billing records with parameters: {contextid}", parameters.ContextId);

            string query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}/{Section}/records?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<CallBillingRecord>(message, cancellationToken)!;
        }

        #endregion

        #region BILLING VALUE METHODS

        /// <summary>
        /// Get billing value after it has been proper registered and accounted
        /// </summary>
        public async Task<decimal?> GetBillingValue(BillingValueRequestParameters parameters, CancellationToken cancellationToken)
        {
            _logger.LogTrace("getting billing value: {contextid}, {linkedid}, {uniqueid}", parameters.ContextId, parameters.LinkedId, parameters.UniqueId);

            string query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}/{Section}/value?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestStruct<decimal>(message, cancellationToken);
        }

        /// <summary>
        /// Get billing values for linked ID
        /// </summary>
        public Task<IEnumerable<BillingValueResponse>> GetValues(Guid contextid, string linkedid, CancellationToken cancellationToken)
        {
            _logger.LogTrace("getting billing values: {contextid}, {linkedid}", contextid, linkedid);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextid.ToString();
            query["linkedid"] = linkedid;

            var uri = new Uri($"{Controller}/{Section}/values?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<BillingValueResponse>(message, cancellationToken)!;
        }

        #endregion

        #region BALANCE METHODS (BalanceControllerInterface)

        /// <inheritdoc cref="BalanceControllerInterface.Notify"/>
        public async Task Notify(BalanceNotifyRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("notifying: {contextid} => {force}", request.ContextId, request.Force);

            string query = request.ToQueryString();
            var uri = new Uri($"{Controller}/{Section}/balance?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Head, uri);            
            await Request(message, cancellationToken);
        }

        /// <inheritdoc cref="BalanceControllerInterface.Patch"/>
        [Authorize(Roles = $"{TelephonyAdminRole.NormalizedName},{SalesManagerRole.NormalizedName}")]
        public async Task Patch(BalancePatchRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("patching: {contextid} => {limit}", request.ContextId, request.Limit);

            var uri = new Uri($"{Controller}/{Section}/balance", UriKind.Relative);
#if NET5_0_OR_GREATER
            var message = new HttpRequestMessage(HttpMethod.Patch, uri);
#else
            var message = new HttpRequestMessage(HttpMethod.Put, uri);
#endif
            message.Content = JsonContent.Create(request, null, _json);
            await Request(message, cancellationToken);
        }

        /// <summary>
        /// Post balance (Administrator role required)
        /// </summary>
        [Authorize(Roles = $"{AdministratorRole.NormalizedName}")]
        public async Task PostBalance(Balance request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("posting balance: {contextid}", request.ContextId);

            var uri = new Uri($"{Controller}/{Section}/balance", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(request, null, _json);
            await Request(message, cancellationToken);
        }

        /// <inheritdoc cref="BalanceControllerInterface.Get"/>
        public async Task<Balance?> Get(Guid contextId, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}/{Section}/balance?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await Request<Balance>(message, cancellationToken);
        }

        /// <inheritdoc cref="BalanceControllerInterface.GetAmount"/>
        public async Task<decimal?> GetAmount(Guid contextId, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextId.ToString();

            var uri = new Uri($"{Controller}/{Section}/amount?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestStruct<decimal>(message, cancellationToken);
        }

        #endregion
    }
}
