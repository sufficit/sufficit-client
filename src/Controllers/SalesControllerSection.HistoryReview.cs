using Sufficit.Sales;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Client.Controllers;

public sealed partial class SalesControllerSection
{
    /// <summary>Reads current authority metadata under the authenticated manager identity.</summary>
    public Task<ServiceHistoryReviewSession?> GetServiceHistoryReviewSession(CancellationToken token)
        => Request<ServiceHistoryReviewSession>(new HttpRequestMessage(HttpMethod.Get,
            new Uri(Controller + "/LegacyService/Session", UriKind.Relative)), token);

    /// <summary>Reads customer-scoped historical rows with the existing finance read entitlement.</summary>
    public Task<IEnumerable<SalesRecord>> GetServiceHistory(RecordSearchParameters search, ServiceHistoryReviewSession session, CancellationToken token)
    {
        if (search.ContextId.GetValueOrDefault() == Guid.Empty || search.Limit.GetValueOrDefault() == 0 || search.Limit > 500)
            throw new ArgumentException("Explicit customer and bounded history are required.");
        var message = HistoryMessage(HttpMethod.Post, Controller + "/Record/Search", session);
        message.Content = JsonContent.Create(search, null, _json);
        return RequestMany<SalesRecord>(message, token);
    }

    /// <summary>Reads the current revision of one historical service before review.</summary>
    public Task<LegacyServiceEditorState?> GetHistoricalServiceEditor(Guid id, ServiceHistoryReviewSession session, CancellationToken token)
        => Request<LegacyServiceEditorState>(HistoryMessage(HttpMethod.Get, Controller + "/LegacyService?id=" + id.ToString("D"), session), token);

    /// <summary>Submits an unchanged retry identity through the dedicated start-only correction endpoint.</summary>
    public Task<LegacyServiceChangeResult?> CorrectHistoricalServiceStart(LegacyServiceChange command, ServiceHistoryReviewSession session, CancellationToken token)
    {
        if (command.CutoverId != session.CutoverId || command.ContractId != Guid.Empty || command.Action != "save" || string.IsNullOrWhiteSpace(command.Reason))
            throw new ArgumentException("Invalid historical correction authority or intent.");
        var message = HistoryMessage(HttpMethod.Post, Controller + "/LegacyService/StartCorrection", session);
        message.Content = JsonContent.Create(command, null, _json);
        return Request<LegacyServiceChangeResult>(message, token);
    }

    private static HttpRequestMessage HistoryMessage(HttpMethod method, string path, ServiceHistoryReviewSession session)
    {
        if (string.IsNullOrWhiteSpace(session.Epoch) || session.SnapshotId == Guid.Empty || session.CutoverId == Guid.Empty)
            throw new ArgumentException("Verified history authority is required.");
        var message = new HttpRequestMessage(method, new Uri(path, UriKind.Relative));
        message.Headers.Add("X-Sufficit-Sales-Epoch", session.Epoch);
        message.Headers.Add("X-Sufficit-Sales-Snapshot", session.SnapshotId.ToString("D"));
        return message;
    }
}
