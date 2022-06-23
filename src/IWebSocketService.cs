using Microsoft.AspNetCore.SignalR.Client;
using Sufficit.Telephony.CheckUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Client
{
    public interface IWebSocketService : ICheckUpMethods
    {
        event EventHandler? OnChanged;

        HubConnectionState State { get; }

        void Test();
    }
}
