﻿using Sufficit.Provisioning;
using Sufficit.Sales;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sufficit.Client
{
    public static class ToQueryStringExtensions
    {
        public static string ToQueryString(this NameValueCollection source)
        {
            var array = from key in source.AllKeys
                        from value in (source.GetValues(key) ?? new string[] { })
                        select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));

            return "?" + string.Join("&", array);
        }

        /// <summary>
        ///     Convert the default call search parameters to a string to query api
        /// </summary>
        public static string ToQueryString(this ICallSearchParameters source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query[nameof(source.IDContext).ToLower()] = source.IDContext.ToString();
            query[nameof(source.Start).ToLower()] = source.Start.ToString("O");

            if (source.End.HasValue)
                query[nameof(source.End).ToLower()] = source.End.Value.ToString("O");

            if (source.Region.HasValue)
                query[nameof(source.Region).ToLower()] = source.Region.Value.ToString();

            if (source.DIDs != null && source.DIDs.Any())
                query[nameof(source.DIDs).ToLower()] = string.Join(",", source.DIDs);

            query[nameof(source.Billed).ToLower()] = source.Billed.ToString();
            query[nameof(source.Answered).ToLower()] = source.Answered.ToString();

            if (source.Limit > 0)
                query[nameof(source.Limit).ToLower()] = source.Limit.ToString();

            if (source.MaxRecords > 0)
                query[nameof(source.MaxRecords).ToLower()] = source.MaxRecords.ToString();

            return query.ToString() ?? string.Empty;
        }


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

        public static string ToQueryString(this ContractSearchParameters source)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (source.ContextId != null)
                query[nameof(source.ContextId).ToLower()] = source.ContextId.ToString();

            if (source.Start != null)
            {
                if (source.Start.Start.HasValue)
                    query["start.start"] = source.Start.Start.Value.ToString("o");

                if (source.Start.End.HasValue)
                    query["start.end"] = source.Start.End.Value.ToString("o");

                if (source.Start.Inclusive)
                    query["start.inclusive"] = source.Start.Inclusive.ToString().ToLower();
            }

            if (source.Expiration != null)
            {
                if (source.Expiration.Start.HasValue)
                    query["expiration.start"] = source.Expiration.Start.Value.ToString("o");

                if (source.Expiration.End.HasValue)
                    query["expiration.end"] = source.Expiration.End.Value.ToString("o");

                if (source.Expiration.Inclusive)
                    query["expiration.inclusive"] = source.Expiration.Inclusive.ToString().ToLower();
            }

            if (source.Filter != null)
                query[nameof(source.Filter).ToLower()] = source.Filter.ToString();

            if (source.Limit > 0)
                query[nameof(source.Limit).ToLower()] = source.Limit.ToString();

            return query.ToString() ?? string.Empty;
        }

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
