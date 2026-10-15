# Charge payment preference contract

Added nullable PreferredPaymentMethod to BillingCheckoutLinkDto and an optional preferredPaymentMethod argument to SetBillingCheckoutLink, appended after CancellationToken for source compatibility. Configure sends the nullable JSON field.

Validation: Blazor Server build and Release publish passed with this project reference; real Blazor component test passed. Values match the API contract: null=no explicit preference, 0=PIX, 1=Card, 2=BankSlip. No provider action is triggered by this setting.
