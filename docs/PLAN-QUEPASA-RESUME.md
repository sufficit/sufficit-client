# Correção de retomada Quepasa e teste no tablet

Objetivo: fazer Iniciar retomar a sessão existente via POST /resume e validar no tablet Lenovo conectado (HA1NP62H).

## Checkpoints
1. Concluído — contrato identificado; tablet disponível via ADB/Chrome DevTools.
2. Concluído — regressão reproduzida (1 passou/1 falhou); após correção os 2 testes passaram e o projeto compilou em net10.0.
3. Em andamento — commit/push main, publicação NuGet automática e deploy limpo do Blazor pelo CI (usa sufficit-client/main).
4. Pendente — validar no tablet real, registrar evidências e limites.

## Restrições e evidências
- Preservar alterações paralelas no Blazor; não publicar árvore suja.
- Não apagar rotas, pareamentos ou dados do tablet.
- Teste: dotnet test test/Sufficit.Client.IntegrationTests.csproj --filter FullyQualifiedName~WhatsAppQuepasaRequestTests.
- Validar URL, método e identidade da sessão no request; pareamento novo continua usando /start com JSON.
- Produção: comparar build publicado e observar request /resume no tablet.
