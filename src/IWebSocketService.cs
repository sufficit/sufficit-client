using Microsoft.AspNetCore.SignalR.Client;
using Sufficit.CheckUp;
using System;
using System.Threading.Tasks;

namespace Sufficit.Client
{
    public interface IWebSocketService : ICheckUpMethods
    {
        event EventHandler? OnChanged;

        HubConnectionState State { get; }

        Task StartAsync();
        System.Collections.Generic.IAsyncEnumerable<CustomerChecklistEvent> CustomerChecklist(
            Guid contextId, string kind, string key, Guid? batchId, System.Threading.CancellationToken cancellationToken);
    }
}
