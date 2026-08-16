using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Sufficit.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Finance
{
    /// <summary>
    /// Keeps bank slip group membership outside the generic websocket service.
    /// </summary>
    internal sealed class BankSlipRealtimeService : IBankSlipRealtimeService, IAsyncDisposable
    {
        private const string ChangedMethod = "BankSlipChanged";
        private const string JoinMethod = "JoinBankSlips";
        private const string LeaveMethod = "LeaveBankSlips";

        private readonly WebSocketService _webSocket;
        private readonly ILogger<BankSlipRealtimeService> _logger;
        private readonly IDisposable _changeHandler;
        private readonly SemaphoreSlim _subscriptionGate = new(1, 1);
        private readonly HashSet<Guid> _contexts = new();
        private readonly CancellationTokenSource _disposeCts = new();
        private bool _restoreWhenConnected;
        private bool _hasConnected;
        private int _disposeRequested;

        public BankSlipRealtimeService(
            WebSocketService webSocket,
            ILogger<BankSlipRealtimeService> logger)
        {
            _webSocket = webSocket;
            _logger = logger;
            _changeHandler = _webSocket.On<BankSlipChange>(ChangedMethod, HandleBankSlipChanged);
            _webSocket.OnChanged += HandleConnectionChanged;
        }

        public event EventHandler? OnConnectionChanged;
        public event EventHandler<BankSlipChange>? OnBankSlipChanged;

        public HubConnectionState State => _webSocket.State;

        private bool IsDisposed => Volatile.Read(ref _disposeRequested) != 0;

        public async Task JoinAsync(
            Guid contextId,
            CancellationToken cancellationToken = default)
        {
            if (contextId == Guid.Empty)
                throw new ArgumentException("A bank slip context is required.", nameof(contextId));
            ThrowIfDisposed();

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _disposeCts.Token);
            linkedCts.Token.ThrowIfCancellationRequested();
            await _webSocket.StartAsync().ConfigureAwait(false);
            if (State != HubConnectionState.Connected)
                throw new InvalidOperationException("The bank slip realtime connection is unavailable.");

            await _subscriptionGate.WaitAsync(linkedCts.Token).ConfigureAwait(false);
            try
            {
                // Group membership belongs to the current SignalR connection and is
                // lost whenever that connection is recreated. Invoke the server even
                // for a desired context we already track so callers can verify that a
                // reconnect (or a previously rejected join) is actually subscribed.
                // AddToGroupAsync is idempotent for the same connection/group pair.
                await _webSocket.InvokeAsync(
                    JoinMethod,
                    new object?[] { contextId },
                    linkedCts.Token).ConfigureAwait(false);
                _contexts.Add(contextId);
            }
            finally
            {
                _subscriptionGate.Release();
            }
        }

        public async Task LeaveAsync(
            Guid contextId,
            CancellationToken cancellationToken = default)
        {
            if (contextId == Guid.Empty || IsDisposed)
                return;

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _disposeCts.Token);
            await _subscriptionGate.WaitAsync(linkedCts.Token).ConfigureAwait(false);
            try
            {
                if (!_contexts.Remove(contextId) || State != HubConnectionState.Connected)
                    return;

                await _webSocket.InvokeAsync(
                    LeaveMethod,
                    new object?[] { contextId },
                    linkedCts.Token).ConfigureAwait(false);
            }
            finally
            {
                _subscriptionGate.Release();
            }
        }

        private void HandleBankSlipChanged(BankSlipChange change)
        {
            if (!IsDisposed)
                OnBankSlipChanged?.Invoke(this, change);
        }

        private async void HandleConnectionChanged(object? sender, EventArgs eventArgs)
        {
            if (IsDisposed)
                return;

            try
            {
                if (State == HubConnectionState.Connected)
                {
                    if (_hasConnected && _restoreWhenConnected)
                        await RestoreSubscriptionsAsync(_disposeCts.Token).ConfigureAwait(false);

                    _hasConnected = true;
                    _restoreWhenConnected = false;
                }
                else if (_hasConnected)
                {
                    _restoreWhenConnected = true;
                }
            }
            catch (OperationCanceledException) when (IsDisposed)
            {
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "Could not restore bank slip realtime subscriptions after reconnecting.");
            }
            finally
            {
                if (!IsDisposed)
                    OnConnectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private async Task RestoreSubscriptionsAsync(CancellationToken cancellationToken)
        {
            await _subscriptionGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                foreach (var contextId in _contexts.ToArray())
                {
                    await _webSocket.InvokeAsync(
                        JoinMethod,
                        new object?[] { contextId },
                        cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                _subscriptionGate.Release();
            }
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(BankSlipRealtimeService));
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposeRequested, 1) != 0)
                return;

            _webSocket.OnChanged -= HandleConnectionChanged;
            _changeHandler.Dispose();
            _disposeCts.Cancel();
            await _subscriptionGate.WaitAsync().ConfigureAwait(false);
            try
            {
                _contexts.Clear();
                OnConnectionChanged = null;
                OnBankSlipChanged = null;
            }
            finally
            {
                _subscriptionGate.Release();
                _subscriptionGate.Dispose();
                _disposeCts.Dispose();
            }
        }
    }
}
