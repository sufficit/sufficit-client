using Microsoft.AspNetCore.Authorization;
using Sufficit.AI;
using Sufficit.Net.Http;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.AI
{
    /// <summary>
    /// Controller section for the AI assistant configuration (bootstrap prompt + guardrails).
    /// </summary>
    public sealed class AIControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/ai";
        private const string AdminRoles = Sufficit.Identity.AdministratorRole.NormalizedName;

        private readonly JsonSerializerOptions _json;

        public AIControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        [Authorize]
        public Task<AIAssistantSettings?> GetAssistantSettings(CancellationToken cancellationToken)
            => Request<AIAssistantSettings>(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"{Controller}/assistant", UriKind.Relative)), cancellationToken);

        [Authorize(Roles = AdminRoles)]
        public Task<AIAssistantSettings?> SaveAssistantSettings(AIAssistantSettings settings, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage(HttpMethod.Put,
                new Uri($"{Controller}/assistant", UriKind.Relative));
            message.Content = JsonContent.Create(settings, null, _json);
            return Request<AIAssistantSettings>(message, cancellationToken);
        }
    }
}
