using Sufficit.Identity;
using System.Net.Http;

namespace Sufficit.Client
{
    public class ProtectedApiBearerTokenHandler : ApiBearerTokenHandler
    {
        public ProtectedApiBearerTokenHandler(ITokenProvider token) : base(token) { }

        protected override bool ShouldAuthenticate(HttpRequestMessage request)
        {
            if (request.Method == HttpMethod.Head)            
                return false;

            switch (request.RequestUri?.AbsolutePath)
            {
                case "/contact":
                case "/health":
                case "/identity/directives":

                case "/notification/contact/validate":
                case "/notification/events":
                case "/notification/subscribe":

                case "/resources/htmltopdf":
                case "/resources/urltopdf":

                case "/telephony/destination/fromasterisk":
                case "/telephony/destination/inuse":
                case "/telephony/eventspanel/endpoints":
                case "/telephony/webcallback": return false;
                default: return true;
            }
        }
    }
}