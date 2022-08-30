using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public class ClientIVRSearchParameters : IVRSearchParameters
    {
        public ClientIVRSearchParameters(Guid ivrId)
        {
            this.IVRId = ivrId;
        }

        public ClientIVRSearchParameters(Guid contextId, string? title) 
        {
            this.ContextId = contextId; 
            this.Title = title; 
        }

        public static implicit operator Guid? (ClientIVRSearchParameters parameters)
            => parameters.IVRId;

        public static implicit operator ClientIVRSearchParameters(Guid ivrId)
            => new ClientIVRSearchParameters(ivrId);

        public static string ToQueryString(IVRSearchParameters parameters)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (parameters.IVRId.HasValue)
                query["ivrid"] = parameters.IVRId.ToString();
            if (parameters.ContextId.HasValue)
                query["contextid"] = parameters.ContextId.ToString();
            if (!string.IsNullOrWhiteSpace(parameters.Title))
                query["title"] = parameters.Title!.ToString();

            return query.ToString() ?? string.Empty;
        }
    }
}
