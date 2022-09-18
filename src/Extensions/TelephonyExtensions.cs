using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Client.Extensions
{
    public static class TelephonyExtensions
    {
        /// <summary>
        /// Convert the default call search parameters to a string to query api
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string ToUriQuery(this ICallSearchParameters parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("idcontext={0}&", parameters.IDContext);
            sb.AppendFormat("start={0}&", parameters.Start.ToString("O"));
            
            if(parameters.End.HasValue)
                sb.AppendFormat("end={0}&", parameters.End?.ToString("O"));

            if (parameters.Region.HasValue)
                sb.AppendFormat("region={0}&", parameters.Region);

            if (parameters.DIDs != null && parameters.DIDs.Any())
                sb.AppendFormat("dids={0}&", parameters.DIDs);

            sb.AppendFormat("billed={0}&", parameters.Billed);
            sb.AppendFormat("answered={0}&", parameters.Answered);

            if (parameters.Limit > 0)
                sb.AppendFormat("limit={0}&", parameters.Limit);

            if (parameters.MaxRecords > 0)
                sb.AppendFormat("maxrecords={0}&", parameters.MaxRecords);

            return sb.ToString();
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
    }
}
