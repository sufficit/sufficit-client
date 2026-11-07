# Consolidação de compromissos e dívida

Consulta e interface consolidam compromisso mensal + saldo devedor por cliente e fonte financeira. Dívida do representante conta uma vez antes da paginação. Avulsas permanecem separadas; sem emissão ou mutação financeira.

Validação: 198 testes de API no código; 170 testes contra DLLs do candidato publicado, incluindo segurança de busca, permissões, checkout, voz e cálculo; 11 testes Blazor. Navegador em 1440/768/390 sem overflow, detalhamento funcional e paginação única oculta. Consulta real de leitura em novembro: 577 clientes, 355 pagadores, compromisso 37320, dívida 9782.01, total 47102.01; nenhum título não resolvido na primeira página.

API publicada em eveo-apps pelo deploy.py oficial. Health HTTP 200; rota protegida HTTP 401 sem credenciais. Hash instalado EndPoints: `66a5b87edf600b554155fc769c12505f64ddd1873861c99dcc8d687568215400`, igual ao candidato testado. Base, EFData e Json também conferidos. Leitura real confirmou duas páginas sem duplicação e seis vínculos explícitos de produto no catálogo. Validação autenticada no navegador de produção não realizada: conector Vault sem autenticação nesta sessão; navegador validado em fixture e serviço real consultado com as DLLs candidatas. Nenhuma cobrança emitida.
