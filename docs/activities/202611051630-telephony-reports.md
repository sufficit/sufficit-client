# Cliente dos relatórios de telefonia

Reports.TelephonyCatalog consulta GET /Telephony/Reports/Catalog;
Reports.TelephonyReport envia request tipado por modelo e retorna snapshot comum,
com cancelamento e planilha opcional da mesma geração. API antiga preservada.
Base mínimo atualizado para 1.26.1105.1926, publicado no nuget.org.

Validação: build Packing e consumidores API/Blazor; 191 testes pertinentes na API,
mais fixture Blazor em 1440/768/390 px. Publicação de pacote ocorre pelo workflow
de push e não comprova que aplicações já usam o cliente novo.

Revisão de integração: Reports possui HttpClient próprio com timeout de 200 segundos
para comportar o limite de 180 segundos da API; demais seções mantêm sua configuração.
O serviço descarta os dois clientes ao encerrar seu escopo.
