# Authenticated activity source requests

Added OperationalActivities.OpenFromEvent for the administrator-only API source-event boundary. Local validation uses the shared Base mapping before sending; requests preserve source identity and never send an actor override or update revision. Requires Base >= 1.26.1006.30.

All configured Client targets built. Two focused synthetic HTTP tests passed: authenticated exact-payload retries and rejection before transport for invalid source identity. No real credentials, operator session or production business mutation were used.

Publication is tracked in PLAN-ACTIVITY-SOURCE-EVENTS.md; source module mappings, reminders and queue UI remain separate pending work.

Publication and three-host API installation completed; final CI, package and installed verification evidence is recorded in PLAN-ACTIVITY-SOURCE-EVENTS.md. The broader module event mappings, reminders and queue UI remain pending.
