using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Sufficit.Identity;
using Sufficit.Net.Http;
using Sufficit.Resources.Whisper;
using Sufficit.Telephony;
using Sufficit.Telephony.Asterisk;
using Sufficit.Telephony.Call;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    /// <summary>
    ///     Telephony Call
    /// </summary>
    public sealed class TelephonyCallControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;
        private const string Section = "calls";

        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _json;

        public TelephonyCallControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
            _logger = cb.Logger;
        }

        #region CALL INFO METHODS

        /// <summary>
        /// Get call information
        /// </summary>
        [Authorize(Roles = $"{ManagerRole.NormalizedName},{AdministratorRole.NormalizedName}")]
        public async Task<IEnumerable<TelephonyCall>> GetCallInfo(CallInfoRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("getting call info: {linkedid}, {uniqueid}, {id}", request.LinkedId, request.UniqueId, request.Id);

            string query = request.ToQueryString();
            var uri = new Uri($"{Controller}/{Section}/info?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestMany<TelephonyCall>(message, cancellationToken) ?? Array.Empty<TelephonyCall>();
        }

        #endregion

        #region CALL SEARCH METHODS

        /// <summary>
        /// Search for call records with specified parameters
        /// </summary>
        [Authorize]
        public async Task<IEnumerable<ICallRecordBasic>> CallSearch(CallSearchParameters parameters, CancellationToken cancellationToken)
        {
            _logger.LogTrace("searching calls with parameters: {contextid}", parameters.ContextId);

            string query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}/{Section}?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestMany<CallRecord>(message, cancellationToken) ?? Array.Empty<CallRecord>();
        }

        #endregion

        #region CALL RECORDS METHODS

        /// <summary>
        /// Get call records by linked ID
        /// </summary>
        [Authorize]
        public async Task<IEnumerable<AsteriskCallDetailsRecord>?> GetRecords(Guid contextid, string linkedid, CancellationToken cancellationToken)
        {
            _logger.LogTrace("getting call records: {contextid}, {linkedid}", contextid, linkedid);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = contextid.ToString();
            query["linkedid"] = linkedid;

            var uri = new Uri($"{Controller}/{Section}/records?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await RequestMany<AsteriskCallDetailsRecord>(message, cancellationToken);
        }

        #endregion

        #region CALL MESSAGE METHODS

        /// <summary>
        /// Start call message
        /// </summary>
        [Authorize]
        public async Task CallMessageStart(CallMessageRequestParameters parameters, CancellationToken cancellationToken)
        {
            _logger.LogTrace("starting call message: {contextid}", parameters.ContextId);

            var uri = new Uri($"{Controller}/{Section}/message", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            await Request(message, cancellationToken);
        }

        #endregion

        #region TRANSCRIPT METHODS

        /// <summary>
        /// Get call transcript
        /// </summary>
        public async Task<WhisperResponse> GetTranscript(TranscriptRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("getting call transcript: {linkedid}", request.LinkedId);

            string query = request.ToQueryString();
            var uri = new Uri($"{Controller}/{Section}/transcript?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await Request<WhisperResponse>(message, cancellationToken) ?? new WhisperResponse();
        }

        #endregion

        #region INSIGHT METHODS

        /// <summary>
        /// Get call insight
        /// </summary>
        [Authorize]
        public async Task GetInsight(string id, CancellationToken cancellationToken)
        {
            _logger.LogTrace("getting call insight: {id}", id);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["id"] = id;

            var uri = new Uri($"{Controller}/{Section}/insight?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            await Request(message, cancellationToken);
        }

        #endregion
    }
}