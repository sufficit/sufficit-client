using Microsoft.AspNetCore.SignalR.Client;
using Sufficit.Campaigns.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers.Campaigns
{
    /// <summary>Live events of the campaigns of a context (requires the campaigns entitlement in that context).</summary>
    public interface ICampaignRealtimeService
    {
        event EventHandler? OnConnectionChanged;

        event EventHandler<CampaignEvent>? OnCampaignEvent;

        HubConnectionState State { get; }

        Task JoinAsync(Guid contextId, CancellationToken cancellationToken = default);

        Task LeaveAsync(Guid contextId, CancellationToken cancellationToken = default);
    }
}
