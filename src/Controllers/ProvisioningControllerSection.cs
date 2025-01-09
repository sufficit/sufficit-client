using Microsoft.AspNetCore.Authorization;
using Sufficit.Net.Http;
using Sufficit.Provisioning;
using Sufficit.Telephony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class ProvisioningControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/provisioning";

        private readonly JsonSerializerOptions _json;

        public ProvisioningControllerSection (IAuthenticatedControllerBase cb) : base(cb)
        {
            _json = cb.Json;
        }

        [Authorize(Roles = "provisioning")]
        public Task Delete(string key, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(key)) 
                throw new ArgumentNullException(nameof(key));

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["macaddress"] = key;

            var uri = new Uri($"{Controller}/device?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }

        [Authorize(Roles = "provisioning")]
        public Task Delete(string macaddress, string key, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(macaddress))
                throw new ArgumentNullException(nameof(macaddress));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["macaddress"] = macaddress;
            query["key"] = key;

            var uri = new Uri($"{Controller}/attribute?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Delete, uri);
            return Request(message, cancellationToken);
        }


        [Authorize(Roles = "provisioning")]
        public Task Update(Device device, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/device", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(device, null, _json);
            return Request(message, cancellationToken);
        }

        [Authorize(Roles = "provisioning")]
        public Task Update(DeviceAttribute device, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/attribute", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(device, null, _json);
            return Request(message, cancellationToken);
        }

        [Authorize(Roles = "provisioning")]
        public Task<IEnumerable<Device>> Search(DeviceSearchParameters parameters, CancellationToken cancellationToken)
        {
            var query = parameters.ToQueryString();
            var uri = new Uri($"{Controller}/device/search?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<Device>(message, cancellationToken);
        }

        [Authorize(Roles = "provisioning")]
        public Task<Device?> ByMAC(string macaddress, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["macaddress"] = macaddress;

            var uri = new Uri($"{Controller}/device?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return Request<Device>(message, cancellationToken);
        }

        [Authorize(Roles = "provisioning")]
        public Task<IEnumerable<DeviceAttribute>> AttributesByMAC(string macaddress, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            query["macaddress"] = macaddress;

            var uri = new Uri($"{Controller}/attributes?{query}", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Get, uri);
            return RequestMany<DeviceAttribute>(message, cancellationToken);
        }
    }
}
