# Scoped sales integration delegation

Separate audit-read and transport-retry grants, without role derivation. API checks explicit customer context; internal snapshots override incoming token claims. No grants or production retry commands created. Current permission probe returns only scope-bound booleans, no customer/event data, and does not replace command authorization. Client metadata requires authentication instead of a global administrator role.

Blazor reads authoritative API decisions, invalidates evidence on authentication changes, hides unauthorized retries and rejects late receipts belonging to a previous identity. Assistant reports current read/retry flags only over loaded evidence. English code/comments; pt-BR/en resources.

Validation: 2 Identity.Core tests, 12 API tests, 25 real-app Blazor tests passed. Identity.Core all targets compiled; EndPoints 33 projects/zero errors; Blazor 20 projects/zero errors. Synthetic browser at 1280/390px passed read-only and denied scope plus existing retry/query/localization checks. Encoding check and source diff checks passed. No authenticated production operator flow claimed.

Rollout/source verification is pending. Internal allowlists/scopes and actual user/customer grant assignments remain untouched. Metrics, event evolution and the other eight module plans remain pending.
