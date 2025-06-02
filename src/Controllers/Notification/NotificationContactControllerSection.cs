using Sufficit.EndPoints;
using Sufficit.Net.Http;
using Sufficit.Notification;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Notification
{
    public sealed class NotificationContactControllerSection : AuthenticatedControllerSection
    {
        private const string Controller = NotificationControllerSection.Controller;
        private const string Prefix = "/contact";

        private readonly JsonSerializerOptions _json;

        public NotificationContactControllerSection(IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        public Task<EndPointResponse<ContactValidationResponse>> Validate(ContactValidationRequest parameters, CancellationToken cancellationToken = default)
        {       
            var uri = new Uri($"{Controller}{Prefix}/validate", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);
            return Request<EndPointResponse<ContactValidationResponse>>(message, cancellationToken)!;
        }

        protected override string[]? AnonymousPaths { get; } = { $"{Controller}{Prefix}/validate" };
    }
}
