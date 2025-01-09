using Sufficit.Net.Http;

namespace Sufficit.Client.Controllers
{
    public sealed class ExchangeControllerSection : AuthenticatedControllerSection
    {
        public const string Controller = "/exchange";

        public ExchangeControllerSection(IAuthenticatedControllerBase cb) : base(cb) 
        {
            Messages = new ExchangeMessagesControllerSection(cb);
        }
    
        public ExchangeMessagesControllerSection Messages { get; }
    }
}
