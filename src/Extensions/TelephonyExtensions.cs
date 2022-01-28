using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.APIClient.Extensions
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
            sb.AppendFormat("limit={0}", parameters.Limit);
            return sb.ToString();
        }
    }
}
