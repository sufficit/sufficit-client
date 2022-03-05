using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
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
            /*
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://identity.sufficit.com.br:5001";
                    options.Audience = "api";
                });
            */
            // importante para incluir propriedades extras ao usuario
            //services.AddScoped<AuthenticationStateProvider, CustomRemoteAuthenticationService>();
            //services.AddScoped<AuthorizationMessageHandler>();

            var provider = services.BuildServiceProvider();
            var configuration = provider.GetRequiredService<IConfiguration>();

            // Definindo o local da configuração global
            // Importante ser dessa forma para o sistema acompanhar as mudanças no arquivo de configuração em tempo real 
            services.Configure<EndPointsAPIOptions>(options => configuration.GetSection(EndPointsAPIOptions.SECTIONNAME));

            // Capturando para uso local
            var endpointApiOptions = configuration.GetSection(EndPointsAPIOptions.SECTIONNAME).Get<EndPointsAPIOptions>() ?? new EndPointsAPIOptions();
            var builder = services.AddHttpClient(endpointApiOptions.ClientId, client => client.BaseAddress = new Uri(endpointApiOptions.BaseUrl));
            var handler = provider.GetService<AuthorizationMessageHandler>();
            if (handler != null) {
                handler.ConfigureHandler(authorizedUrls: GetAuthorizedUrls(endpointApiOptions));
                builder.AddHttpMessageHandler(hn => handler);
            }

            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(endpointApiOptions.ClientId));

            services.TryAddTransient<APIClientService>();
            services.TryAddTransient<IWebSocketService, WebSocketService>();

            return services;
        }

        public static IEnumerable<string> GetAuthorizedUrls(EndPointsAPIOptions options)
        {
            if (options != null)
            {
                yield return $"{options.BaseUrl}";
            }
        }
    }
}
