# Charge com pagamentos parciais — 14/10/2026

## Entrega local

Uma charge consolidada recebe vários pagamentos: valor congelado por tentativa, saldo reservado no banco em transação, pagamentos confirmados somados sem duplicação, itens quitados somente ao completar o total. Checkout permite escolher valor e consultar tentativas anteriores. A autorização recorrente e pagamentos parciais não podem competir pelo mesmo saldo.

Blazor: Vendas → Cobranças, recebido/saldo/histórico por forma de pagamento e itens; configuração recolhida; AI somente leitura. Novos nomes em inglês com Charge; contratos do provedor preservados.

## Evidência

186 testes Checkout + 11 EndPoints passaram; Blazor compilou sem erros. Regressão Razor reproduzida antes da correção e corrigida. Migração executada duas vezes em MariaDB 11.4.13 isolado, com unicidade e integridade de pagamentos verificadas. Checkout real renderizado com dados sintéticos e inspecionado em Chromium 390/768/1280/1920 sem overflow, nos estados formulário e pagamento pendente.

Não houve publicação, DDL de produção nem pagamento/assinatura real. Navegador autenticado Blazor não validado. Push continua retido até migração coordenada e resolução do guard oficial de áudio. Alterações de navegação de outra frente foram preservadas e ficam fora destes commits.

## Limites operacionais

Mínimo atual de R$ 5 por tentativa, sem deixar saldo residual menor que R$ 5. Reservas inconclusivas não são liberadas pela simples expiração local. Estornos/cancelamento e reconciliação de autorizações exigem operação assistida. Links históricos continuam válidos com a política assinada original.

Plano detalhado: `sufficit-endpoints/docs/PLAN-UNIFIED-CUSTOMER-CHECKOUT.md`.
