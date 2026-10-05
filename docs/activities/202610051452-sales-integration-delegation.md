# Scoped sales integration delegation

Separate audit-read and transport-retry grants, without role derivation. API checks explicit customer context; internal snapshots override incoming token claims. No grants or production retry commands created. Current permission probe returns only scope-bound booleans, no customer/event data, and does not replace command authorization. Client metadata requires authentication instead of a global administrator role.

Blazor reads authoritative API decisions, invalidates evidence on authentication changes, hides unauthorized retries and rejects late receipts belonging to a previous identity. Assistant reports current read/retry flags only over loaded evidence. English code/comments; pt-BR/en resources.

Validation: 2 Identity.Core tests, 12 API tests, 25 real-app Blazor tests passed. Identity.Core all targets compiled; EndPoints 33 projects/zero errors; Blazor 20 projects/zero errors. Synthetic browser at 1280/390px passed read-only and denied scope plus existing retry/query/localization checks. Encoding check and source diff checks passed. No authenticated production operator flow claimed.

Rollout/source verification is pending. Internal allowlists/scopes and actual user/customer grant assignments remain untouched. Metrics, event evolution and the other eight module plans remain pending.


Source checkpoint: 2ec66d605b2cfff925ddc084ac872552b1f9d803. Reviewed files committed in a real task worktree and fast-forwarded to the original main branch without switching it. Publication acceptance is still in progress.

Remote main is 2ec66d6; Client package workflow 37352978387 succeeded after the new Identity.Core package became available.

Final delivery acceptance: Blazor server CI 37353651522 and shared-package CI 37353651527 succeeded for 84b64b9. All three hosts and public proxy are Healthy with 1.26.1005.1814+84b64b9; five installed application/dependency hashes are identical across hosts. Full API CI passed 680 tests, Blazor encoding CI passed four tests. The ninth authorization delivery is complete; metrics, authenticated production user acceptance and the eight following modules remain pending. Actual grant assignments and production event retries were not performed.
