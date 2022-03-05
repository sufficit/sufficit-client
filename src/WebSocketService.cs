using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.EndPoints.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Client
{
    public class WebSocketService : IWebSocketService
    {
        private readonly IOptions<EndPointsAPIOptions> _options;
        private readonly ILogger _logger;
        public readonly HubConnection _connection;

        public WebSocketService(IOptions<EndPointsAPIOptions> options, ILogger<WebSocketService> logger)
        {
            _options = options; 
            _logger = logger;

            _connection = new HubConnectionBuilder()
                .WithUrl($"{_options.Value.BaseUrl}/ws")

                // Só começa a reconectar se iniciou a 1ª conexão com sucesso
                .WithAutomaticReconnect(new TimeSpan[]{ TimeSpan.FromSeconds(10) })
                .Build();

            _connection.Reconnected     += _connection_Reconnected;
            _connection.Reconnecting    += _connection_Reconnecting;
            _connection.Closed          += _connection_Closed;

            

            _logger.LogTrace("WebSocketService Instantiated.");
        }

        public async void Test()
        {
            try
            {
                await _connection.StartAsync();
                OnChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error on trying to connect");
            }
        }

        private async Task _connection_Closed(Exception? arg)
        {
            await Task.Yield();
            _logger.LogDebug("Closed");
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        private async Task _connection_Reconnecting(Exception? arg)
        {
            await Task.Yield();
            _logger.LogDebug("Reconnecting");
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        private async Task _connection_Reconnected(string? arg)
        {
            await Task.Yield();
            _logger.LogDebug("Reconnected");
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? OnChanged;

        public HubConnectionState State => _connection.State;
    }
}
