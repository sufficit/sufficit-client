using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sufficit.Client.Controllers.Campaigns
{
    public static class CampaignServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the campaign realtime adapter over the shared websocket transport.
        /// Call <c>AddSufficitEndPointsAPI</c> before enabling this feature.
        /// </summary>
        public static IServiceCollection AddSufficitCampaignRealtime(this IServiceCollection services)
        {
            services.TryAddScoped<ICampaignRealtimeService, CampaignRealtimeService>();
            return services;
        }
    }
}
