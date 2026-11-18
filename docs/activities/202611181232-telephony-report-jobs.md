# Relatórios de telefonia assíncronos

O Client ganhou operações HTTP de criação, recuperação, status, resultado e
cancelamento. A notificação `TelephonyReportJobChanged` reutiliza a conexão
SignalR autenticada. A geração síncrona permanece disponível para consumidores
existentes.

A compilação passou nos três targets. O fixture Blazor validou o transporte
HTTP e o recebimento do aviso no hub de teste.
