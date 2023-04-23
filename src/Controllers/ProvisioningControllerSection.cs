﻿using Microsoft.AspNetCore.Authorization;
using Sufficit.Gateway.ReceitaNet;
using Sufficit.Identity;
using Sufficit.Provisioning;
using Sufficit.Telephony;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class ProvisioningControllerSection : ControllerSection
    {
        public const string Controller = "/provisioning";

        public ProvisioningControllerSection(APIClientService service) : base(service) { }

        [Authorize(Roles = "provisioning")]
        public Task Update(Device device, CancellationToken cancellationToken)
        {
            var uri = new Uri($"{Controller}/device", UriKind.Relative);
            var message = new HttpRequestMessage(HttpMethod.Post, uri);
            message.Content = JsonContent.Create(device, null, jsonOptions);
            return Request(message, cancellationToken);
        }

        [Authorize(Roles = "provisioning")]
        public Task<IEnumerable<Device>> Search(DeviceSearchParameters parameters, CancellationToken cancellationToken)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
            if (parameters.ContextId != null)
                query["contextid"] = parameters.ContextId.ToString();

            if (parameters.ExtensionId != null)
                query["extensionid"] = parameters.ExtensionId.ToString();

            if (parameters.MACAddress != null)
                query["macaddress"] = parameters.MACAddress.ToString();

            if (parameters.IPAddress != null)
                query["ipaddress"] = parameters.IPAddress.ToString();

            if (parameters.Limit > 0)
                query["limit"] = parameters.Limit.ToString();

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
    }
}