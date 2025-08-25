using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sufficit.Client.Resources;
using Sufficit.EndPoints.Configuration;
using Sufficit.Identity;
using Sufficit.Resources;
using System;
using System.Net.Http;

namespace Sufficit.Client
{
    /// <summary>
    /// Classe que controla os metodos referentes a API de Endpoints da Sufficit
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Inclui toda a configuração para sincronia com a API de EndPoints da Sufficit
        /// </summary>
        public static IServiceCollection AddSufficitEndPointsAPI(this IServiceCollection services)
        {
            services.AddOptions<EndPointsAPIOptions>();

            var provider = services.BuildServiceProvider(false);
            var configuration = provider.GetRequiredService<IConfiguration>();

            // Definindo o local da configuração global
            // Importante ser dessa forma para o sistema acompanhar as mudanças no arquivo de configuração em tempo real 
            services.Configure<EndPointsAPIOptions>(configuration.GetSection(EndPointsAPIOptions.SECTIONNAME));

            // Capturando para uso local
            var options = configuration.GetSection(EndPointsAPIOptions.SECTIONNAME).Get<EndPointsAPIOptions>() ?? new EndPointsAPIOptions();

            // If not added previously, using default token provider
            services.TryAddScoped<ITokenProvider, HttpContextTokenProvider>();

            services.AddScoped<APIClientService>();
            services.AddScoped<IWebSocketService, WebSocketService>();
            services.TryAddScoped<IPDFTool, RemotePDFTool>();

            return services;
        }

        /* // example of policy handler for httpclient
        .AddPolicyHandler(GetRetryPolicy())
        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
        */
    }
}
