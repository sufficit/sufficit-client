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

        public async Task<ContactValidationResponse> Validate(ContactValidationRequest parameters, CancellationToken cancellationToken = default)
        {
            var result = new ContactValidationResponse() 
            { 
                Success = false,
                Channel = parameters.Channel,
                Destination = parameters.Destination,
            };           

            var uri = new Uri($"{Controller}{Prefix}/validate", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(parameters, null, _json);

            using var response = await SendAsync(message, cancellationToken);
            try
            {
                await response.EnsureSuccess(cancellationToken);
                result.Success = true;
            } 
            catch (HttpRequestException ex)
            {
                try
                {
                    var epresp = JsonSerializer.Deserialize<EndPointResponse>(ex.Message);
                    if (epresp != null)
                    {
                        result.Success = epresp.Success;
                        result.Message = epresp.Message;
                        result.Link = epresp.Link;
                        result.Data = epresp.Data;
                        return result;
                    }
                }
                catch { }
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return result;

            if (response.Content.Headers.ContentLength > 0 && response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                var response_object = await response.Content.ReadFromJsonAsync<ContactValidationResponse>(_json, cancellationToken);
                if (response_object != null) result = response_object;
            }

            return result;
        }

        protected override string[]? AnonymousPaths { get; } = { $"{Controller}{Prefix}/validate" };
    }
}
