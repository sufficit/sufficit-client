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
    }
}
