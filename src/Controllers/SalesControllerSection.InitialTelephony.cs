using Sufficit.Sales;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers;

public sealed partial class SalesControllerSection
{
    public Task<InitialTelephonyState?> GetInitialTelephonyState(Guid contextId, ServiceHistoryReviewSession session, CancellationToken token)
    {
        if (contextId == Guid.Empty) throw new ArgumentException("Explicit customer is required.");
        return Request<InitialTelephonyState>(HistoryMessage(HttpMethod.Get,
            Controller + "/LegacyService/Initialization?contextid=" + contextId.ToString("D"), session), token);
    }
    public Task<LegacyServiceChangeResult?> InitializeTelephony(LegacyServiceChange command, ServiceHistoryReviewSession session, CancellationToken token)
    {
        if (command.CutoverId != session.CutoverId || command.Action != "create" || command.OperationId == Guid.Empty)
            throw new ArgumentException("Verified initialization authority and intent are required.");
        var message = HistoryMessage(HttpMethod.Post, Controller + "/LegacyService/Initialization", session);
        message.Content = JsonContent.Create(command, null, _json);
        return Request<LegacyServiceChangeResult>(message, token);
    }
}
