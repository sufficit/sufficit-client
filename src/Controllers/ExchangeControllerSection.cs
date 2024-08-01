using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Sufficit.Client.Controllers.Telephony;
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
    public sealed class ExchangeControllerSection : ControllerSection
    {
        public const string Controller = "/exchange";

        public ExchangeControllerSection(APIClientService service) : base(service)
        {
            Messages = new ExchangeMessagesControllerSection(service);
        }
    
        public ExchangeMessagesControllerSection Messages { get; }
    }
}
