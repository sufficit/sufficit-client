using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sufficit.EndPoints.Configuration;
using System;

namespace Sufficit.APIClient.Extensions
{
    /// <summary>
    /// Classe que controla os metodos referentes a API de Endpoints da Sufficit
    /// </summary>
    public static class EndPointsExtensions
    {
        /// <summary>
        /// Inclui toda a configuração para sincronia com a API de EndPoints da Sufficit
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddSufficitEndPointsAPI(this IServiceCollection services)
        {
            IConfiguration? configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            if (configuration == null) throw new ArgumentNullException("configuration");

            // Definindo o local da configuração global
            // Importante ser dessa forma para o sistema acompanhar as mudanças no arquivo de configuração em tempo real 
            services.Configure<EndPointsAPIOptions>(options => configuration.GetSection(EndPointsAPIOptions.SectionName).Bind(options));

            // Capturando para uso local
            var endpointApiOptions = configuration.GetSection(EndPointsAPIOptions.SectionName).Get<EndPointsAPIOptions>();
            services
                .AddHttpClient(endpointApiOptions.ClientId, client => client.BaseAddress = new Uri(endpointApiOptions.BaseUrl))
                .AddHttpMessageHandler(sp =>
                {
                    var serv = sp.GetService<AuthorizationMessageHandler>();
                    if (serv == null) throw new ArgumentException("AuthorizationMessageHandler service not found");

                    return serv.ConfigureHandler(authorizedUrls: new[] { endpointApiOptions.BaseUrl });
                });

            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(endpointApiOptions.ClientId));

            services.AddTransient<APIClientService>();
            services.TryAddTransient(sp => sp.GetRequiredService<APIClientService>());

            return services;
        }
    }
}
