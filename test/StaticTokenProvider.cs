using Sufficit.Identity;
using System.Threading.Tasks;

namespace Sufficit.Client.IntegrationTests
{
    /// <summary>
    /// Simple token provider for integration tests that returns a fixed token value
    /// </summary>
    public class StaticTokenProvider : ITokenProvider
    {
        private readonly string _token;

        public StaticTokenProvider(string token)
        {
            _token = token;
        }

        public ValueTask<string?> GetTokenAsync() => new ValueTask<string?>(_token);
    }
}
