using Microsoft.AspNetCore.Authorization;
using Sufficit.Audio;
using Sufficit.Client.Controllers.Audio;
using Sufficit.Net.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public sealed class AudioControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/audio";

        public AudioControllerSection(IAuthenticatedControllerBase cb) : base(cb) 
        {
            Background = new BackgroundControllerSection(cb);
            Mix = new MixControllerSection(cb);
        }

        public BackgroundControllerSection Background { get; }

        public MixControllerSection Mix { get; }
    }
}
