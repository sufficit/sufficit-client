using Microsoft.Extensions.Logging;
using Sufficit.Gateway.ReceitaFederal;
using Sufficit.Net.Http;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Gateway
{
    public sealed class ReceitaFederalControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = "/Gateway";
        private const string Prefix = "/ReceitaFederal";

        private readonly ILogger _logger;

        public ReceitaFederalControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _logger = cb.Logger;
        }

        public Task<Person?> Person(string document, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace("person by document: {document}", document);

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["document"] = document;

            var uri = new Uri($"{Controller}{Prefix}/person?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<Person>(message, cancellationToken);
        }
    }
}