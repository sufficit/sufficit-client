using Sufficit.Identity;
using System.Net.Http;

namespace Sufficit.Client
{
    public class ProtectedApiBearerTokenHandler : ApiBearerTokenHandler
    {
        public ProtectedApiBearerTokenHandler(ITokenProvider token) : base(token) { }

        protected override bool ShouldAuthenticate(HttpRequestMessage request)
        {
            switch (request.RequestUri?.AbsolutePath)
            {
                case "/health":
                case "/contact":
                case "/identity/directives":
                case "/telephony/eventspanel/endpoints":
                case "/telephony/webcallback": return false;
                default: return true;
            }
        }
    }
}