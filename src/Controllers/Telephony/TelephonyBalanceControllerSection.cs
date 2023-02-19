using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Telephony
{
    public sealed class TelephonyBalanceControllerSection : ControllerSection
    {
        private const string Controller = TelephonyControllerSection.Controller;

        public TelephonyBalanceControllerSection(APIClientService service) : base(service) { }

        public async Task Notify(Guid idcontext, bool force, CancellationToken cancellationToken)
        {
            logger.LogTrace("notifying: {idcontext} => {force}", idcontext, force);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["idcontext"] = idcontext.ToString();
            query["force"] = force.ToString();

            var uri = new Uri($"{Controller}/balance?{query}", UriKind.Relative);
            var request = new HttpRequestMessage(HttpMethod.Head, uri);            
            var response = await httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
