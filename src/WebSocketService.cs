using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sufficit.CheckUp;
using Sufficit.EndPoints.Configuration;
using Sufficit.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client
{
    public class WebSocketService : IWebSocketService, IAsyncDisposable
    {
        private readonly IOptions<EndPointsAPIOptions> _options;
        private readonly ILogger _logger;
        private readonly ITokenProvider _tokenProvider;
        private readonly CancellationTokenSource _disposeCts = new();
        private readonly SemaphoreSlim _connectionGate = new(1, 1);
        private int _disposeRequested;
        public readonly HubConnection _connection;

        public WebSocketService(IOptions<EndPointsAPIOptions> options, ILogger<WebSocketService> logger, ITokenProvider tokenProvider)
        {
            _options = options;
            _logger = logger;
            _tokenProvider = tokenProvider;

            _connection = new HubConnectionBuilder()
                .WithUrl($"{_options.Value.BaseUrl}/ws", httpConnectionOptions =>
                {
                    httpConnectionOptions.AccessTokenProvider = async () => await _tokenProvider.GetTokenAsync();
                })

                // Só começa a reconectar se iniciou a 1ª conexão com sucesso
                .WithAutomaticReconnect(new TimeSpan[] { TimeSpan.FromSeconds(10) })
                .Build();

            _connection.Reconnected += _connection_Reconnected;
            _connection.Reconnecting += _connection_Reconnecting;
            _connection.Closed += _connection_Closed;



            _logger.LogTrace("WebSocketService Instantiated.");
        }

        public async Task StartAsync()
        {
            if (IsDisposed)
                return;

            try
            {
                await _connectionGate.WaitAsync(_disposeCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (IsDisposed)
            {
                return;
            }

            try
            {
                // A HubConnection can only be started once at a time. Components can
                // render more than once while connecting, so make the operation idempotent.
                if (IsDisposed || _connection.State != HubConnectionState.Disconnected)
                    return;

                await _connection.StartAsync(_disposeCts.Token).ConfigureAwait(false);
                NotifyChanged();
            }
            catch (OperationCanceledException) when (IsDisposed)
            {
                // Disposal cancels a pending connection attempt; it is not an error.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error on trying to connect");
            }
            finally
            {
                _connectionGate.Release();
            }
        }

        private Task _connection_Closed(Exception? arg)
        {
            _logger.LogDebug("Closed");
            NotifyChanged();
            return Task.CompletedTask;
        }

        private Task _connection_Reconnecting(Exception? arg)
        {
            _logger.LogDebug("Reconnecting");
            NotifyChanged();
            return Task.CompletedTask;
        }

        private Task _connection_Reconnected(string? arg)
        {
            _logger.LogDebug("Reconnected");
            NotifyChanged();
            return Task.CompletedTask;
        }

        public event EventHandler? OnChanged;

        public HubConnectionState State => _connection.State;

        private bool IsDisposed => Volatile.Read(ref _disposeRequested) != 0;

        private void NotifyChanged()
        {
            if (!IsDisposed)
                OnChanged?.Invoke(this, EventArgs.Empty);
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposeRequested, 1) != 0)
                return;

            _disposeCts.Cancel();
            await _connectionGate.WaitAsync().ConfigureAwait(false);
            try
            {
                _connection.Reconnected -= _connection_Reconnected;
                _connection.Reconnecting -= _connection_Reconnecting;
                _connection.Closed -= _connection_Closed;
                OnChanged = null;

                await _connection.DisposeAsync().ConfigureAwait(false);
            }
            finally
            {
                _connectionGate.Release();
                _disposeCts.Dispose();
            }
        }

        #region IMPLEMENTS INTERFACE CHECKUP METHODS

        public IAsyncEnumerable<CheckUpStepInfo> CheckUpOutBoundRoutes(Guid ContextId, CancellationToken cancellationToken)
            => _connection.StreamAsync<CheckUpStepInfo>("CheckUpOutBoundRoutes", ContextId, cancellationToken);

        #endregion
    }
}
