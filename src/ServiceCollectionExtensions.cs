using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sufficit.EndPoints.Configuration;
using System;

namespace Sufficit.Client
{
    /// <summary>
    /// Classe que controla os metodos referentes a API de Endpoints da Sufficit
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Inclui toda a configuração para sincronia com a API de EndPoints da Sufficit
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddSufficitEndPointsAPI(this IServiceCollection services)
        {
            services.AddOptions<EndPointsAPIOptions>();

            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();

            // Definindo o local da configuração global
            // Importante ser dessa forma para o sistema acompanhar as mudanças no arquivo de configuração em tempo real 
            services.Configure<EndPointsAPIOptions>(configuration.GetSection(EndPointsAPIOptions.SECTIONNAME));

            // Capturando para uso local
            var options = configuration.GetSection(EndPointsAPIOptions.SECTIONNAME).Get<EndPointsAPIOptions>() ?? new EndPointsAPIOptions();
            
            services.AddTransient<ProtectedApiBearerTokenHandler>();
            services.AddHttpClient(options.ClientId, client => client.BaseAddress = new Uri(options.BaseUrl))
                .AddHttpMessageHandler<ProtectedApiBearerTokenHandler>();

            services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(options.ClientId));

            services.TryAddTransient<APIClientService>();
            services.TryAddTransient<IWebSocketService, WebSocketService>();

            return services;
        }

        public static IApplicationBuilder UseSufficitEndPointsAPI(this IApplicationBuilder app) 
        {


            return app;
        }
    }
}
