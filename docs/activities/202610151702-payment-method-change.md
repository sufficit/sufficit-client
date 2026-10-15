# Change payment method after preparing a charge

User correction: hiding the payment alternatives made checkout confusing. A prepared provider charge is not evidence that the invoice was paid.

## Delivered behavior

PIX, boleto and card remain visible and enabled. Selecting a different method cancels the original unpaid one-off attempt through the private API. Only confirmed deletion releases the local reservation. The new method is selected without automatically creating or paying another charge. The customer can then prepare that payment normally.

The API checks provider ID, intent reference, unpaid status and absence of subscription/installment binding before deletion, and reads back deleted/unpaid state afterwards. Confirmed/received/risk-analysis, recurring and unrelated payments are rejected. An already-deleted unpaid attempt is handled idempotently. Local receipt guards preserve settlement during races; delayed non-settlement webhooks do not reopen a cancelled reservation. Other partial-payment reservations remain intact. Network errors/timeouts retain the reservation.

## Evidence and limits

- 195 Checkout tests passed, including cancellation, inconclusive retry, settled refusal, partial reservation preservation and stale webhook behavior.
- 38 API/authentication tests passed, including ownership/recurring restrictions, concurrent confirmation, authenticated cancellation route and rejection without the integration key.
- Actual Asaas Sandbox: synthetic R$5 card charge PENDING, DELETE confirmed and GET readback PENDING/deleted=true; no card data or payment was submitted. IDs/evidence stay in protected workspace tmp.
- Release publishes passed with existing warnings. API and Checkout installed with preserved settings and healthy services; installed API hash verified against artifact.
- Live malformed cancellation request returned 401 without key and 400 with key, before any provider mutation.
- Browser 390/1280 px: three enabled methods, existing card selection retained, clear change explanation, no redundant “Ver pagamento”, keyboard focus works, no overflow or JavaScript errors. Browser review focused controls without changing selection or creating/cancelling a TOGO payment.

TOGO's current provider attempt was preserved. Production recurring price changes still await Asaas tokenization permission; this work does not enable that capability.

Rollback backups: /opt/sufficit-checkout.before-payment-change-20261015 and /opt/sufficit-endpoints.before-a62b59523f35471ebfcd4905a843ca90. Screenshots/JSON use payment-choice-live and payment-change prefixes in the protected workspace tmp.
