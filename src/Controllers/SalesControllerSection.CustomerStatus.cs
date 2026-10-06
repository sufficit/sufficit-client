using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Sufficit.Sales;

namespace Sufficit.Client.Controllers;

public sealed partial class SalesControllerSection
{
    /// <summary>Reads the commercial customer state through the authorized Sales API.</summary>
    public Task<CustomerStatusRecord?> GetCustomerStatus(Guid contextId, CancellationToken token)
    {
        if (contextId == Guid.Empty) throw new ArgumentException("An explicit customer context is required.");
        return Request<CustomerStatusRecord>(new HttpRequestMessage(HttpMethod.Get,
            new Uri($"{Controller}/Customers/{contextId:D}/Status", UriKind.Relative)), token);
    }

    /// <summary>Reads bounded immutable source application receipts after an acceptance sequence.</summary>
    public Task<IEnumerable<CustomerStatusHistory>> GetCustomerStatusHistory(Guid contextId, long afterSequence, int limit, CancellationToken token)
    {
        if (contextId == Guid.Empty || afterSequence < 0 || limit < 1 || limit > 100) throw new ArgumentException("Invalid customer history query.");
        return RequestMany<CustomerStatusHistory>(new HttpRequestMessage(HttpMethod.Get, new Uri(
            $"{Controller}/Customers/{contextId:D}/Status/History?afterSequence={afterSequence.ToString(CultureInfo.InvariantCulture)}&limit={limit.ToString(CultureInfo.InvariantCulture)}", UriKind.Relative)), token);
    }

    /// <summary>Applies a reviewed source classification, preserving the request identity across retries.</summary>
    public Task<CustomerStatusHistory?> ImportCustomerStatus(CustomerStatusTransitionRequest request, CancellationToken token)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        request.Validate();
        var message = new HttpRequestMessage(HttpMethod.Post, new Uri($"{Controller}/Customers/{request.ContextId:D}/Status/Import", UriKind.Relative));
        message.Content = JsonContent.Create(request, null, _json);
        return Request<CustomerStatusHistory>(message, token);
    }
}
