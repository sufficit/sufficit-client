# Retomada de sessões Quepasa

O botão Iniciar no Blazor chama Start(string sessionId) do cliente. Esse método
usava /start, reservado pela API a novo pareamento com JSON, em vez de /resume.
Corrigida apenas a URL da sobrecarga de sessão existente, preservando o pareamento.

## Validação
- Teste HTTP com handler em memória, sem operações em produção.
- Antes: 1 teste passou (novo pareamento); 1 falhou (/start recebido em vez de /resume).
- Depois: 2 testes passaram; compilação net10.0 concluída. Warnings existentes nas dependências.
- Comando: dotnet test test/Sufficit.Client.IntegrationTests.csproj --filter FullyQualifiedName~WhatsAppQuepasaRequestTests.
- Tablet Lenovo TB-J706F acessível via ADB e Chrome DevTools; página WhatsApp aberta.
- Publicação e teste final no tablet acompanhados em ../PLAN-QUEPASA-RESUME.md.
