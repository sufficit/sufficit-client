using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Sufficit.Audio;
using Sufficit.Client.Controllers.Audio;
using Sufficit.Client.Controllers.Telephony;
using Sufficit.Identity;
using Sufficit.Net.Http;
using Sufficit.Resources.TTS;
using Xunit;

namespace Sufficit.Client.IntegrationTests.Resources;

public sealed class TtsAuthorizationTests
{
    [Fact]
    public async Task TtsGenerationRequiresAnAccessToken()
    {
        var section = new TTSControllerSection(CreateControllerBase());

        await Assert.ThrowsAsync<UnauthenticatedExpection>(() =>
            section.Meta(new TTSRequest { Text = "teste" }, CancellationToken.None));
    }

    [Fact]
    public async Task MixGenerationRequiresAnAccessToken()
    {
        var section = new MixControllerSection(CreateControllerBase());

        await Assert.ThrowsAsync<UnauthenticatedExpection>(() =>
            section.FromTTS(new AudioMixRequestFromTTS(), CancellationToken.None));
    }

    private static AuthenticatedControllerBase CreateControllerBase()
        => new(
            new NoTokenProvider(),
            new HttpClient(new UnexpectedRequestHandler()) { BaseAddress = new Uri("https://api.test") },
            new JsonSerializerOptions(JsonSerializerDefaults.Web),
            NullLogger.Instance);

    private sealed class NoTokenProvider : ITokenProvider
    {
        public ValueTask<string?> GetTokenAsync() => ValueTask.FromResult<string?>(null);
    }

    private sealed class UnexpectedRequestHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
    }
}
