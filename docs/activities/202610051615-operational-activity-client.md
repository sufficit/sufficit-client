# Operational activities API Client

Adds APIClientService.Activities with authenticated revision commands, bounded context-scoped reads and immutable history. Client validates explicit identifiers, UTC deadlines and limits before dispatch. Request identifiers must remain stable across network retries. The API supplies the actor; Client never submits actor identities. Uses the new Base contract package as its minimum dependency.

Validation: Client built for every configured target with zero errors (21 existing dependency warnings). Server-side tests verify access denial, trusted actor derivation, sanitized business conflicts and infrastructure error propagation. Package publication is distinct from installed Blazor UI: this delivery introduces no queue page, delegated grants or automatic reminders.
