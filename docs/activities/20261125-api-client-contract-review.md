# Revisão de contratos do cliente .NET e do EndPoints

Inventário de 25/11/2026 a partir do [OpenAPI v2 de produção](https://endpoints.sufficit.com.br/swagger/v2/swagger.json). A contagem considera método e caminho. Os prefixos v1 e v2 agrupam operações de vários domínios. O inventário mede a superfície da API. A revisão funcional cobriu os casos descritos abaixo.

| Prefixo | Operações publicadas |
| --- | ---: |
| access | 7 |
| ai | 4 |
| api | 17 |
| audio | 9 |
| billing | 8 |
| cadences | 12 |
| campaigns | 17 |
| communication | 4 |
| contact | 22 |
| documentation | 3 |
| exchange | 4 |
| finance | 66 |
| gateway | 79 |
| identity | 23 |
| mcp | 21 |
| notification | 14 |
| operations | 7 |
| provisioning | 7 |
| public | 1 |
| sales | 117 |
| support | 4 |
| system | 3 |
| telephony | 415 |
| test | 1 |
| utils | 1 |
| v1 | 4 |
| v2 | 45 |
| weatherforecast | 1 |
| web | 2 |

## Contratos revisados

- DID. O cliente lê e remove o destino e associa a fila a partir do estado esperado do servidor. O GET Destination também cobre a leitura exposta pelo alias GET QueueDestination. A consulta de atividade distingue zero explícito (histórico completo) do padrão de 30 dias.
- Exchange. A consulta de mensagens por referência usa a pesquisa pública autenticada com ReferenceId e o limite padrão do servidor. A consulta de recibos usa id na query e desserializa os campos timestamp e description. Os testes verificam uma resposta contendo recibo.
- Provisionamento. Os sete pares de verbo e rota têm métodos no SDK para dispositivos, busca e atributos. Conferência estática de query e corpo.
- Access. As sete operações publicadas de EntitlementPresets têm métodos na seção do SDK. A consulta legada UserDirectives está implementada no controlador e oculta do OpenAPI em Release.
- Finance e Operations. Testes de requisição cobrem pagamentos recentes e eventos de origem de atividades. Não foi encontrado wrapper para ActivityReminders.
- Áudio. O SDK tem Background e Mix. A conversão AnyToAsteriskWavFormat não foi localizada no SDK.
- Communication. As quatro operações de WhatsApp de agente de voz estão no OpenAPI. Não foi localizada uma seção correspondente no SDK.
- Telefonia de ramais. O OpenAPI publica Preparation e o cliente de ramais não oferece a operação. A seção de interconexões já inclui testes de registro síncrono e assíncrono.
- Outros prefixos. Foi feita uma triagem estática de caminhos e verbos nos fontes do SDK. Métodos que constroem caminhos dinamicamente, aliases, operações internas e controladores ocultos exigem revisão individual. A ausência de uma correspondência textual não comprova uma falha em execução.

Nenhuma escrita de negócio foi usada para medir cobertura.
