using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Sufficit.Sales;

namespace Sufficit.Client.Controllers;

public partial class SalesControllerSection
{
    /// <summary>Reads authoritative prepaid renewal and awarded resources for a single authorized contract.</summary>
    public Task<ServicePrepaidDetails?> GetServicePrepaidDetails(Guid contractId, CancellationToken token)
        => Request<ServicePrepaidDetails>(new HttpRequestMessage(HttpMethod.Get,
            Controller + "/ServiceCollections/PrepaidDetails?contractId=" + contractId.ToString("D")), token);

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
    public Task<ServicePrepaidPlanQuote?> PreviewServicePlan(ServicePrepaidPlanCommand command, CancellationToken token)
        => CollectionCommand<ServicePrepaidPlanQuote>("PlanPreview", command, token);
    public Task<ServicePrepaidPlanOperation?> SubmitServicePlan(ServicePrepaidPlanCommand command, CancellationToken token)
        => CollectionCommand<ServicePrepaidPlanOperation>("PlanCommand", command, token);
    public Task<ServicePrepaidPlanOperation?> GetServicePlanOperation(Guid contractId, Guid operationId, CancellationToken token)
        => Request<ServicePrepaidPlanOperation>(new HttpRequestMessage(HttpMethod.Get,
            Controller + "/ServiceCollections/PlanOperation?contractId=" + contractId.ToString("D") + "&operationId=" + operationId.ToString("D")), token);
    public Task<ServiceCollectionPage?> GetServiceCollectionPeriods(string referenceDate, int leadDays, int groupDays, Guid? contextId, int offset, CancellationToken token, Guid? responsibleContextId = null)
    {
        var uri = Controller + "/ServiceCollections?consolidate=true&referenceDate=" + Uri.EscapeDataString(referenceDate)
            + "&leadDays=" + leadDays + "&groupDays=" + groupDays + "&offset=" + offset
            + (contextId.HasValue ? "&contextId=" + contextId.Value.ToString("D") : "")
            + (responsibleContextId.HasValue ? "&responsibleContextId=" + responsibleContextId.Value.ToString("D") : "");
        return Request<ServiceCollectionPage>(new HttpRequestMessage(HttpMethod.Get, uri), token);
    }
    public Task<ServiceCollectionPeriodHistory?> GetServiceCollectionPeriodHistory(Guid contextId, Guid recipientId, string month, long afterRevision, CancellationToken token)
        => Request<ServiceCollectionPeriodHistory>(new HttpRequestMessage(HttpMethod.Get, Controller + "/ServiceCollections/PeriodHistory?contextId=" + contextId.ToString("D")
            + "&responsibleContextId=" + recipientId.ToString("D") + "&month=" + Uri.EscapeDataString(month) + "&afterRevision=" + afterRevision), token);
    public Task<ServiceCollectionPeriodReceipt?> SetServiceCollectionPeriodContact(ServiceCollectionPeriodCommand command, CancellationToken token)
        => CollectionCommand<ServiceCollectionPeriodReceipt>("PeriodContact", command, token);
    private Task<T?> CollectionCommand<T>(string path, object command, CancellationToken token) where T : class, new()
    {
        var message = new HttpRequestMessage(HttpMethod.Post, Controller + "/ServiceCollections/" + path)
            { Content = JsonContent.Create(command, null, _json) };
        return Request<T>(message, token);
    }
}
