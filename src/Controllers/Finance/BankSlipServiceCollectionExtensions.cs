using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sufficit.Client.Controllers.Finance
{
    /// <summary>
    /// Registers client-side services owned by the bank slip module.
    /// </summary>
    public static class BankSlipServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the bank slip realtime adapter over the shared websocket transport.
        /// Call <c>AddSufficitEndPointsAPI</c>
        /// before enabling this feature.
        /// </summary>
        public static IServiceCollection AddSufficitBankSlipRealtime(
            this IServiceCollection services)
        {
            services.TryAddScoped<IBankSlipRealtimeService, BankSlipRealtimeService>();
            return services;
        }
    }
}
