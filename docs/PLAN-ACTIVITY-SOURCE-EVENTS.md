# Activity creation from source events

Objective: expose an authenticated source-event boundary that opens independent collaborator work once, preserving immutable receipts and later operator edits.

1. COMPLETED — Define and validate a source request contract and its stable command mapping in Base.
2. COMPLETED — Wire the administrator-only API and authenticated Client; verify exact replay, changed replay, actor/context isolation and retention against the relational store.
3. IN PROGRESS — Publish verified packages/API and record installed evidence.

Scope: explicit OperationalActivityRequested events. Do not infer task creation from every commercial change, replay the historical integration backlog, invent an automation actor, or send production reminders. Generic automatic module event mappings and durable reminder dispatch remain pending in operational queue checkpoint 3; queue UI remains checkpoint 4. This delivery creates no activity during deployment.

English identifiers and inline documentation; future UI localized with pt-BR fallback. No schema changes are needed: reuse activity command receipts and context-scoped origin keys.

Two authenticated transport tests passed; all Client targets built. Requires Base >= 1.26.1006.30 because that package contains the source event contract. Base publication CI 37394280132 succeeded; public feed propagation is being verified before Client publication.
