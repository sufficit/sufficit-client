using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Sufficit.Finance.Credits;

namespace Sufficit.Client.Controllers.Finance;

public sealed partial class FinanceControllerSection
{
    public Task<CreditOverview?> GetCreditAccount(Guid contextId, Guid productId, CancellationToken ct)
        => Request<CreditOverview>(new HttpRequestMessage(HttpMethod.Get, new Uri($"/Finance/Credits/Account?contextId={contextId:D}&productId={productId:D}", UriKind.Relative)), ct);
    public Task<IEnumerable<CreditVoucher>> GetCreditVouchers(CancellationToken ct)
        => RequestMany<CreditVoucher>(new HttpRequestMessage(HttpMethod.Get, new Uri("/Finance/Credits/Vouchers", UriKind.Relative)), ct);
    public Task<IEnumerable<CreditResource>> GetCreditResources(CancellationToken ct)
        => RequestMany<CreditResource>(new HttpRequestMessage(HttpMethod.Get, new Uri("/Finance/Credits/Resources", UriKind.Relative)), ct);
    public Task<IEnumerable<CreditProduct>> GetCreditProducts(CancellationToken ct)
        => RequestMany<CreditProduct>(new HttpRequestMessage(HttpMethod.Get, new Uri("/Finance/Credits/Products", UriKind.Relative)), ct);
    public Task<CreditVoucher?> IssueCreditVoucher(CreditVoucher voucher, CancellationToken ct)
        => Request<CreditVoucher>(new HttpRequestMessage(HttpMethod.Post, new Uri("/Finance/Credits/Vouchers", UriKind.Relative))
        { Content = JsonContent.Create(voucher, null, _json) }, ct);
    public Task DisableCreditVoucher(Guid id, CancellationToken ct)
        => RequestStruct<bool>(new HttpRequestMessage(HttpMethod.Post, new Uri($"/Finance/Credits/Vouchers/{id:D}/Disable", UriKind.Relative)), ct);
}
