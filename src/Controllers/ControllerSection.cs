using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.EndPoints.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers
{
    public abstract class ControllerSection
    {
        protected readonly IOptionsMonitor<EndPointsAPIOptions> ioptions;
        protected readonly IHttpClientFactory factory;
        protected readonly ILogger logger;
        protected readonly JsonSerializerOptions jsonOptions;
        private Action<bool>? _healthy;

        public ControllerSection(IOptionsMonitor<EndPointsAPIOptions> ioptions, IHttpClientFactory factory, ILogger logger, JsonSerializerOptions jsonOptions)
        {
            this.ioptions = ioptions;
            this.factory = factory;
            this.logger = logger;
            this.jsonOptions = jsonOptions;
        }

        public ControllerSection(APIClientService service)
        {
            this.ioptions = service.ioptions;
            this.factory = service.factory;
            this.logger = service.logger;
            this.jsonOptions = service.jsonOptions;

            this._healthy = service.Healthy;
        }

        #region TRICKS 

        protected HttpClient httpClient
            => factory.Configure(options);

        protected EndPointsAPIOptions options
            => ioptions.CurrentValue;

        #endregion

        protected Task<IEnumerable<T>> RequestManyStruct<T>(HttpRequestMessage message, CancellationToken cancellationToken)
            => RequestManyInternal<T>(message, cancellationToken);

        protected Task<IEnumerable<T>> RequestMany<T>(HttpRequestMessage message, CancellationToken cancellationToken) where T : class, new()
            => RequestManyInternal<T>(message, cancellationToken);

        private async Task<IEnumerable<T>> RequestManyInternal<T>(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            using var response = await httpClient.SendAsync(message, cancellationToken);
            await response.EnsureSuccess(cancellationToken);

            // updating healthy for this controller
            _healthy?.Invoke(true);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return Enumerable.Empty<T>();

            return await response.Content.ReadFromJsonAsync<IEnumerable<T>>(jsonOptions, cancellationToken) ?? Enumerable.Empty<T>();
        }

        protected async Task<T?> RequestStruct<T>(HttpRequestMessage message, CancellationToken cancellationToken) where T : struct
        {
            using var response = await httpClient.SendAsync(message, cancellationToken);
            await response.EnsureSuccess(cancellationToken);

            // updating healthy for this controller
            _healthy?.Invoke(true);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await response.Content.ReadFromJsonAsync<T>(jsonOptions, cancellationToken);
        }

        protected async Task<T?> Request<T> (HttpRequestMessage message, CancellationToken cancellationToken) where T : class, new()
        {
            using var response = await httpClient.SendAsync(message, cancellationToken);
            await response.EnsureSuccess(cancellationToken);

            // updating healthy for this controller
            _healthy?.Invoke(true);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await response.Content.ReadFromJsonAsync<T>(jsonOptions, cancellationToken);
        }

        protected async Task<byte[]?> RequestBytes (HttpRequestMessage message, CancellationToken cancellationToken)
        {
            using var response = await httpClient.SendAsync(message, cancellationToken);
            await response.EnsureSuccess(cancellationToken);

            // updating healthy for this controller
            _healthy?.Invoke(true);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return null;

            return await response.Content.ReadAsByteArrayAsync();
        }

        protected async Task Request(HttpRequestMessage message, CancellationToken cancellationToken)
        {
            using var response = await httpClient.SendAsync(message, cancellationToken);
            await response.EnsureSuccess(cancellationToken);

            // updating healthy for this controller
            _healthy?.Invoke(true);
        }
    }
}
