using Microsoft.AspNetCore.SignalR.Client;
using Sufficit.Finance;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Finance
{
    /// <summary>
    /// Provides the realtime subscription used only by the bank slip module.
    /// </summary>
    public interface IBankSlipRealtimeService
    {
        /// <summary>
        /// Raised when the underlying realtime connection changes state.
        /// </summary>
        event EventHandler? OnConnectionChanged;

        /// <summary>
        /// Raised after the server reports a change to a subscribed bank slip.
        /// </summary>
        event EventHandler<BankSlipChange>? OnBankSlipChanged;

        /// <summary>
        /// Gets the current state of the shared realtime connection.
        /// </summary>
        HubConnectionState State { get; }

        /// <summary>
        /// Joins the bank slip group associated with a tenant context.
        /// </summary>
        Task JoinAsync(Guid contextId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Leaves the bank slip group associated with a tenant context.
        /// </summary>
        Task LeaveAsync(Guid contextId, CancellationToken cancellationToken = default);
    }
}
