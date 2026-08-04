# Bank slip V2 authenticated client

## Summary

`FinanceControllerSection.BankSlipV2` exposes the parallel bank slip API to
trusted application clients.

The section supports:

- asynchronous issuance with an `Idempotency-Key`;
- lookup, search and operational statistics;
- tenant settings read/update;
- public capability access enable/disable;
- durable cancellation requests.

Resource identifiers and filters use query strings in accordance with the
Sufficit API convention. A `204 No Content` response is represented as a null
resource/result by the authenticated controller base.

## Realtime boundary

`IWebSocketService` remains transport-oriented and does not expose finance
types or bank slip group names. The bank slip module consumes
`IBankSlipRealtimeService`, which owns the `BankSlipChanged`, `JoinBankSlips`
and `LeaveBankSlips` SignalR protocol, restores tenant subscriptions after a
reconnection and shares the same scoped connection registered by
`AddSufficitEndPointsAPI`. Applications opt into the finance-specific adapter
with `AddSufficitBankSlipRealtime`; the generic endpoint client registration
does not depend on the bank slip module.
