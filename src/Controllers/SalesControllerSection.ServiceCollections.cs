using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Sufficit.Sales;

namespace Sufficit.Client.Controllers;

public partial class SalesControllerSection
{
    /// <summary>Reads a bounded collection portfolio. A missing context does not imply an authorization grant.</summary>
    public Task<ServiceCollectionPage?> GetServiceCollections(string referenceDate, int leadDays, Guid? contextId, int offset, CancellationToken token, Guid? responsibleContextId = null)
    {
        var uri = Controller + "/ServiceCollections?referenceDate=" + Uri.EscapeDataString(referenceDate)
            + "&leadDays=" + leadDays + "&offset=" + offset + (contextId.HasValue ? "&contextId=" + contextId.Value.ToString("D") : "")
            + (responsibleContextId.HasValue ? "&responsibleContextId=" + responsibleContextId.Value.ToString("D") : "");
        return Request<ServiceCollectionPage>(new HttpRequestMessage(HttpMethod.Get, uri), token);
    }
    /// <summary>Submits the exact reviewed recipient command.</summary>
    public Task<ServiceCollectionProfile?> SetServiceCollectionRecipient(ServiceCollectionProfileCommand command, CancellationToken token)
        => CollectionCommand<ServiceCollectionProfile>("Recipient", command, token);
    /// <summary>Changes an existing prepaid binding's explicit opt-in.</summary>
    public Task<ServiceCollectionAutomationReceipt?> SetServiceCollectionAutomation(ServiceCollectionAutomationCommand command, CancellationToken token)
        => CollectionCommand<ServiceCollectionAutomationReceipt>("Automation", command, token);
    public Task<ServiceCollectionFollowupPage?> GetServiceCollectionFollowup(Guid contractId, long afterRevision, CancellationToken token)
        => Request<ServiceCollectionFollowupPage>(new HttpRequestMessage(HttpMethod.Get,
            Controller + "/ServiceCollections/Followup?contractId=" + contractId.ToString("D") + "&afterRevision=" + afterRevision), token);
    public Task<ServiceCollectionFollowupReceipt?> SetServiceCollectionFollowup(ServiceCollectionFollowupCommand command, CancellationToken token)
        => CollectionCommand<ServiceCollectionFollowupReceipt>("Followup", command, token);
    private Task<T?> CollectionCommand<T>(string path, object command, CancellationToken token) where T : class, new()
    {
        var message = new HttpRequestMessage(HttpMethod.Post, Controller + "/ServiceCollections/" + path)
            { Content = JsonContent.Create(command, null, _json) };
        return Request<T>(message, token);
    }
}
