using System.Collections.Specialized;
using System.Web;

namespace Sufficit.APIClient
{
    public static class Utils
    {
        public static string ToQueryString(NameValueCollection nvc)
        {
            var array = from key in nvc.AllKeys
                        from value in (nvc.GetValues(key) ?? new string[] { }) 
                        select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));

            return "?" + string.Join("&", array);
        }
    }
}
