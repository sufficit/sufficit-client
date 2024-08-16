using Sufficit.EndPoints;
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
    public sealed class NotificationContactControllerSection : ControllerSection
    {
        private const string Controller = NotificationControllerSection.Controller;
        private const string Prefix = "/contact";

        public NotificationContactControllerSection(APIClientService service) : base(service) { }    

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
            message.Content = JsonContent.Create(parameters, null, jsonOptions);

            using var response = await httpClient.SendAsync(message, cancellationToken);
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
                var response_object = await response.Content.ReadFromJsonAsync<ContactValidationResponse>(jsonOptions, cancellationToken);
                if (response_object != null) result = response_object;
            }

            return result;
        }
    }
}
