# Publicação integrada dos presets e alterações pendentes

Autorização: usuário solicitou em 07/10/2026 merge de tudo pendente na main em todos os projetos envolvidos e deploy para teste. Inclui as alterações locais já presentes nesses sete projetos; o app público sufficit-identity permanece fora da publicação de modelos de negócio.

## Checkpoints (ordem de execução)

1. CONCLUÍDO — Inventariar branches/PRs e alterações, conferir regras e scripts oficiais, resolver dependências de publicação.
2. CONCLUÍDO — Consolidar o conjunto autorizado na main local; branches conciliadas e backups dos checkouts preservados. API: 763 testes; pré-pago: 29; Blazor focados: 53.
3. EM ANDAMENTO — Integrar na main remota, publicar pacotes na ordem de dependências e verificar os workflows. Base/Identity.Core publicados .1427; Client aguarda indexação NuGet; EFData CI em execução. Validar regressões restantes do Blazor antes do push/deploy.
4. PENDENTE — Aplicar migração de presets com backup e executar deploy oficial de endpoints e Blazor.
5. PENDENTE — Verificar saúde/versões e catálogo publicado; registrar entrega e concluir planos anteriores.

## Evidência de aceitação

Main remota com commits integrados, dependências disponíveis, tabelas e dois presets com balanceview, serviços saudáveis e versão correspondente ao artefato publicado. Não remover trabalho nem branches com alterações não enviadas. Preservar claims existentes. Revalidar o build completo antes do deploy.
