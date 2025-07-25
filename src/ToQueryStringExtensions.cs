﻿using Sufficit.Audio;
using Sufficit.Contacts;
using Sufficit.Exchange;
using Sufficit.Provisioning;
using Sufficit.Reports;
using Sufficit.Sales;
using Sufficit.Telephony;
using Sufficit.Telephony.Audio;
using Sufficit.Telephony.Call;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using static Sufficit.Constants;

namespace Sufficit.Client
{
    public static class ToQueryStringExtensions
    {
        public static string ToQueryString(this NameValueCollection source)
        {
            var array = from key in source.AllKeys
                        from value in (source.GetValues(key) ?? Array.Empty<string>())
                        select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));

            return "?" + string.Join("&", array);
        }
        
        #region AUDIO - BackgroundAudioSearchParameters

        public static string ToQueryString(this BackgroundAudioSearchParameters source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (source.Title != null)
            {
                query[$"{nameof(source.Title).ToLower()}.{nameof(source.Title.Text).ToLower()}"] = source.Title.Text;
                query[$"{nameof(source.Title).ToLower()}.{nameof(source.Title.ExactMatch).ToLower()}"] = source.Title.ExactMatch.ToString().ToLower();
            }

            if (source.Description != null)
            {
                query[$"{nameof(source.Description).ToLower()}.{nameof(source.Description.Text).ToLower()}"] = source.Description.Text;
                query[$"{nameof(source.Description).ToLower()}.{nameof(source.Description.ExactMatch).ToLower()}"] = source.Description.ExactMatch.ToString().ToLower();
            }

            return query.ToString() ?? string.Empty;
        }

        #endregion
        #region CONTACTS - AttributeSearchParameters

        public static string ToQueryString(this AttributeSearchParameters source, NameValueCollection? collection = null)
        {
            var query = collection ?? System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (source.ContactId != null)
                query[nameof(source.ContactId).ToLower()] = source.ContactId.ToString();

            if (source.Value != null)
            {
                query["value.text"] = source.Value.Text;
                query["value.exactmatch"] = source.Value.ExactMatch.ToString().ToLower();
                if (source.Value.Keys != null)
                {
                    foreach (var key in source.Value.Keys)
                        query.Add("value.keys", key.ToLower());
                }

            }

            if (source.Description != null)
            {
                query["description.text"] = source.Description.Text;
                query["description.exactmatch"] = source.Description.ExactMatch.ToString().ToLower();
                if (source.Description.Keys != null)
                {
                    foreach (var key in source.Description.Keys)
                        query.Add("description.keys", key.ToLower());
                }
            }

            return query.ToString() ?? string.Empty;
        }

        #endregion
        #region CONTACTS - AttributeWithKeysSearchParameters

        public static string ToQueryString(this AttributeWithKeysSearchParameters source, NameValueCollection? collection = null)
        {
            var query = collection ?? System.Web.HttpUtility.ParseQueryString(string.Empty);

            // AttributeWithKeysSearchParameters
            if (source.Keys != null)
            {
                foreach (var key in source.Keys)
                    query.Add(nameof(source.Keys).ToLower(), key.ToLower());
            }

            return (source as AttributeSearchParameters).ToQueryString(query);
        }

        #endregion
        #region CONTACTS - ContactSearchParameters

        public static string ToQueryString(this ContactSearchParameters source, NameValueCollection? collection = null)
        {
            var query = collection ?? System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (source.ContextId != null)
                query[nameof(source.ContextId).ToLower()] = source.ContextId.ToString();

            if (source.Limit > 0)
                query[nameof(source.Limit).ToLower()] = source.Limit.ToString();

            return (source as AttributeWithKeysSearchParameters).ToQueryString(query);
        }

        #endregion
        #region EXCHANGE - ReadReceiptSearchParameters

        public static string ToQueryString(this ReadReceiptSearchParameters source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(source.Id).ToLower()] = source.Id.ToString();

            if (source.Start.HasValue)
                query[nameof(source.Start).ToLower()] = source.Start.Value.ToString(DATETIMEFORMAT);

            if (source.Limit.HasValue)
                query[nameof(source.Limit).ToLower()] = source.Limit.ToString();

            return query.ToString() ?? string.Empty;
        }

        #endregion
        #region PROVISIONING - DeviceSearchParameters

        public static string ToQueryString(this DeviceSearchParameters source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (source.ContextId != null)
                query[nameof(source.ContextId).ToLower()] = source.ContextId.ToString();

            if (source.ExtensionId != null)
                query[nameof(source.ExtensionId).ToLower()] = source.ExtensionId.ToString();

            if (source.MACAddress != null)
                query[nameof(source.MACAddress).ToLower()] = source.MACAddress.ToString();

            if (source.IPAddress != null)
                query[nameof(source.IPAddress).ToLower()] = source.IPAddress.ToString();

            if (source.Filter != null)
                query[nameof(source.Filter).ToLower()] = source.Filter.ToString();

            if (source.Order != null)
            {
                query["order.property"] = source.Order.Property;
                query["order.descending"] = source.Order.Descending.ToString();
            }

            if (source.Limit > 0)
                query[nameof(source.Limit).ToLower()] = source.Limit.ToString();

            return query.ToString() ?? string.Empty;
        }

        #endregion
        #region TELEPHONY - AUDIO - AudioSearchParameters

        public static string ToQueryString(this AudioSearchParameters source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (source.AudioId.HasValue)
                query["audioId"] = source.AudioId.ToString();
            if (source.ContextId.HasValue)
                query["contextId"] = source.ContextId.ToString();
            if (!string.IsNullOrWhiteSpace(source.Title))
                query["title"] = source.Title!.ToString();

            return query.ToString() ?? string.Empty;
        }

        #endregion
        #region TELEPHONY - BILLING - BalanceNotifyRequest

        public static string ToQueryString(this BalanceNotifyRequest source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            query[nameof(source.ContextId).ToLower()] = source.ContextId.ToString();

            if (source.Force)
                query[nameof(source.Force).ToLower()] = source.Force.ToString().ToLower();

            return query.ToString() ?? string.Empty;
        }

        #endregion
        #region TELEPHONY - BILLING - BillingSearchParameters

        public static string ToQueryString(this BillingSearchParameters source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (source.ContextId.HasValue)
                query[nameof(source.ContextId).ToLower()] = source.ContextId.Value.ToString();

            if (source.Start.HasValue)
                query[nameof(source.Start).ToLower()] = source.Start.Value.ToString(DATETIMEFORMAT);

            if (source.End.HasValue)
                query[nameof(source.End).ToLower()] = source.End.Value.ToString(DATETIMEFORMAT);

            if (!string.IsNullOrWhiteSpace(source.UniqueId))
                query[nameof(source.UniqueId).ToLower()] = source.UniqueId;

            if (source.Extension != null)
            {
                query[$"{nameof(source.Extension).ToLower()}.{nameof(source.Extension.Text).ToLower()}"] = source.Extension.Text;
                query[$"{nameof(source.Extension).ToLower()}.{nameof(source.Extension.ExactMatch).ToLower()}"] = source.Extension.ExactMatch.ToString().ToLower();
            }

            return query.ToString() ?? string.Empty;
        }

        #endregion
        #region TELEPHONY - BILLING - BillingValueRequestParameters

        public static string ToQueryString(this BillingValueRequestParameters source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            query[nameof(source.ContextId).ToLower()] = source.ContextId.ToString();

            if (!string.IsNullOrWhiteSpace(source.LinkedId))
                query[nameof(source.LinkedId).ToLower()] = source.LinkedId;

            if (!string.IsNullOrWhiteSpace(source.UniqueId))
                query[nameof(source.UniqueId).ToLower()] = source.UniqueId;

            return query.ToString() ?? string.Empty;
        }

        #endregion
        #region TELEPHONY - CALL - CallInfoRequest

        public static string ToQueryString(this CallInfoRequest source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrWhiteSpace(source.LinkedId))
                query[nameof(source.LinkedId).ToLower()] = source.LinkedId;

            if (!string.IsNullOrWhiteSpace(source.UniqueId))
                query[nameof(source.UniqueId).ToLower()] = source.UniqueId;

            if (!string.IsNullOrWhiteSpace(source.Id))
                query[nameof(source.Id).ToLower()] = source.Id;

            return query.ToString() ?? string.Empty;
        }

        #endregion
        #region TELEPHONY - CALL - TranscriptRequest

        public static string ToQueryString(this TranscriptRequest source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            query[nameof(source.LinkedId).ToLower()] = source.LinkedId;

            if (!string.IsNullOrWhiteSpace(source.GGML) && source.GGML != "small")
                query[nameof(source.GGML).ToLower()] = source.GGML;

            if (source.Force)
                query[nameof(source.Force).ToLower()] = source.Force.ToString().ToLower();

            return query.ToString() ?? string.Empty;
        }

        #endregion
        #region TELEPHONY - DestinationSearchParameters

        public static string ToQueryString(this DestinationSearchParameters source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["contextid"] = source.ContextId.ToString();

            if (!string.IsNullOrWhiteSpace(source.Filter))
                query["filter"] = source.Filter;

            if (source.Limit.HasValue)
                query["limit"] = source.Limit.ToString();

            return query.ToString() ?? string.Empty;
        }

        #endregion
        #region TELEPHONY - EndPointPropertyRequest

        public static string ToQueryString(this EndPointPropertyRequest source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            query[nameof(source.Key).ToLower()] = source.Key.ToString();

            if (source.ContextId != Guid.Empty)
                query[nameof(source.ContextId).ToLower()] = source.ContextId.ToString("N");

            if (source.EndPointId.HasValue)
                query[nameof(source.EndPointId).ToLower()] = source.EndPointId.Value.ToString("N");

            if (source.Value != null)
                query[nameof(source.Value).ToLower()] = source.Value;

            return query.ToString() ?? string.Empty;
        }

        #endregion
        #region TELEPHONY - ICallSearchParameters

        /// <summary>
        ///     Convert the default call search parameters to a string to query api
        /// </summary>
        public static string ToQueryString(this ICallSearchParameters source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(source.ContextId).ToLower()] = source.ContextId.ToString();
            query[nameof(source.Start).ToLower()] = source.Start.ToString(DATETIMEFORMAT);

            if (source.End.HasValue)
                query[nameof(source.End).ToLower()] = source.End.Value.ToString(DATETIMEFORMAT);

            if (source.Region.HasValue)
                query[nameof(source.Region).ToLower()] = source.Region.Value.ToString();

            if (source.DIDs != null && source.DIDs.Any())
                query[nameof(source.DIDs).ToLower()] = string.Join(",", source.DIDs);

            if (source.Billed.HasValue)
                query[nameof(source.Billed).ToLower()] = source.Billed.Value.ToString().ToLower();

            if (source.Answered.HasValue)
                query[nameof(source.Answered).ToLower()] = source.Answered.Value.ToString().ToLower();

            if (source.Limit > 0)
                query[nameof(source.Limit).ToLower()] = source.Limit.ToString();

            if (source.MaxRecords > 0)
                query[nameof(source.MaxRecords).ToLower()] = source.MaxRecords.ToString();

            return query.ToString() ?? string.Empty;
        }

        #endregion
        #region REPORTS - ReportRequestParameters

        public static string ToQueryString (this ReportRequestParameters source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (source.Id.HasValue && source.Id.Value != Guid.Empty)
                query[nameof(source.Id).ToLower()] = source.Id.Value.ToString("N");

            if (source.Await.HasValue)
                query[$"{nameof(source.Await).ToLower()}"] = source.Await.Value.ToString().ToLower();

            if (source.ModelId.HasValue && source.ModelId.Value != Guid.Empty)
                query[nameof(source.ModelId).ToLower()] = source.ModelId.Value.ToString("N");

            // Missing extra parameters

            return (source as ReportParameters).ToQueryString(query);
        }

        #endregion
        #region REPORTS - ReportParameters

        public static string ToQueryString (this ReportParameters source, NameValueCollection? query = null)
        {
            query ??= System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (source.ContextId.HasValue && source.ContextId.Value != Guid.Empty)
                query[nameof(source.ContextId).ToLower()] = source.ContextId.Value.ToString("N");

            if (source.ProviderId.HasValue && source.ProviderId.Value != Guid.Empty)
                query[nameof(source.ProviderId).ToLower()] = source.ProviderId.Value.ToString("N");

            if (source.Start.HasValue && source.Start.Value > DateTime.MinValue)
                query[$"{nameof(source.Start).ToLower()}"] = source.Start.Value.ToString(DATETIMEFORMAT);

            if (source.End.HasValue && source.End.Value >  DateTime.MinValue)
                query[$"{nameof(source.End).ToLower()}"] = source.End.Value.ToString(DATETIMEFORMAT);

            if (source.OffSet.HasValue)
                query[$"{nameof(source.OffSet).ToLower()}"] = source.OffSet.Value.ToString();


            return query.ToString() ?? string.Empty;
        }

        #endregion
        #region SALES - ContractSearchParameters

        public static string ToQueryString(this ContractSearchParameters source, NameValueCollection? collection = null)
        {
            var query = collection ?? System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (source.ContextId != null)
                query[nameof(source.ContextId).ToLower()] = source.ContextId.ToString();

            if (source.Start != null)
            {
                if (source.Start.Start.HasValue)
                    query["start.start"] = source.Start.Start.Value.ToString(DATETIMEFORMAT);

                if (source.Start.End.HasValue)
                    query["start.end"] = source.Start.End.Value.ToString(DATETIMEFORMAT);

                if (source.Start.Inclusive)
                    query["start.inclusive"] = source.Start.Inclusive.ToString().ToLower();
            }

            if (source.Expiration != null)
            {
                if (source.Expiration.Start.HasValue)
                    query["expiration.start"] = source.Expiration.Start.Value.ToString(DATETIMEFORMAT);

                if (source.Expiration.End.HasValue)
                    query["expiration.end"] = source.Expiration.End.Value.ToString(DATETIMEFORMAT);

                if (source.Expiration.Inclusive)
                    query["expiration.inclusive"] = source.Expiration.Inclusive.ToString().ToLower();
            }

            if (source.Filter != null)
            {
                query[$"{nameof(source.Filter).ToLower()}.{nameof(source.Filter.Text).ToLower()}"] = source.Filter.Text;
                query[$"{nameof(source.Filter).ToLower()}.{nameof(source.Filter.ExactMatch).ToLower()}"] = source.Filter.ExactMatch.ToString().ToLower();
            }

            if (source.Limit > 0)
                query[nameof(source.Limit).ToLower()] = source.Limit.ToString();

            return query.ToString() ?? string.Empty;
        }

        #endregion

        /* // using POST for now, so no needed to querystring
         
        public static string ToQueryString(this EndPointSearchParameters source, NameValueCollection? collection = null)
        {
            var query = collection ?? System.Web.HttpUtility.ParseQueryString(string.Empty);

            if (source.Id.HasValue)
                query[nameof(source.Id).ToLower()] = source.Id.ToString();

            if (source.UserId.HasValue)
                query[nameof(source.UserId).ToLower()] = source.UserId.ToString();

            if (source.ContextId.HasValue)
                query[nameof(source.ContextId).ToLower()] = source.ContextId.ToString();

            if (source.Filter != null)
            {
                query["filter.text"] = source.Filter.Text;
                query["filter.exactmatch"] = source.Filter.ExactMatch.ToString();
            }

            if (source.Title != null)
            {
                query["title.text"] = source.Title.Text;
                query["title.exactmatch"] = source.Title.ExactMatch.ToString();
            }

            if (source.Description != null)
            {
                query["description.text"] = source.Description.Text;
                query["description.exactmatch"] = source.Description.ExactMatch.ToString();
            }

            if (source.Limit > 0)
                query[nameof(source.Limit).ToLower()] = source.Limit.ToString();

            return query.ToString() ?? string.Empty;
        }
        */
        /*
        public static string ToQueryString(this DIDSearchParameters source)
        {
            var sb = new StringBuilder();
            if (source.ContextId.HasValue)
                sb.AppendFormat("contextid={0}&", source.ContextId);

            if (source.ProviderId.HasValue)
                sb.AppendFormat("providerid={0}&", source.ProviderId);


            var query = System.Web.HttpUtility.ParseQueryString(sb.ToString());
            return query.ToString();
        }
        */
    }
}
